using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BizMall.Data.DBContexts;
using BizMall.Data.Repositories.Abstract;
using BizMall.Models.CompanyModels;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace BizMall.Data.Repositories.Concrete
{
    public class RepositoryCategory : RepositoryBase, IRepository, IRepositoryCategory
    {
        private CategoryType categoryType;

        public RepositoryCategory(ApplicationDbContext ctx, IOptions<AppSettings> settings) : base(ctx)
        {
            categoryType = settings.Value.CategoryType;
        }

        public Category GetCategoryByName(string entitlecategory)
        {
            return _ctx.Categories
                        .Where(c => c.EnTitle == entitlecategory&&c.CategoryType==categoryType)
                        .FirstOrDefault();
        }

        public Category GetCategoryByName(string entitlecategory, CategoryType categoryType)
        {
            return _ctx.Categories
                .Where(c => c.EnTitle == entitlecategory && c.CategoryType == categoryType)
                .FirstOrDefault();
        }

        public IQueryable<Category> Categories(CategoryType ct)
        {
            return _ctx.Categories.Where(c => c.CategoryType == ct);
        }

        public IQueryable<Category> Categories()
        {
            return _ctx.Categories.Where(c => c.CategoryType == categoryType); 
        }

        public IQueryable<Category> ParentCategories(CategoryType curCategoryType)
        {
            var cct = curCategoryType == 0 ? categoryType : curCategoryType;
            return _ctx.Categories.Where(c => c.CategoryType == cct && c.ParentCategory==null);
        }

        public List<string> SitemapCategories()
        {
            var Categories = _ctx.Categories.Where(c => c.CategoryType == categoryType);
            List<string> sitemapCategories = new List<string>();

            foreach(var topcat in Categories)
            {
                //определяем родительская она или нет
                if (topcat.CategoryId == null)
                {
                    bool isParent = false;

                    foreach(var cat in Categories)
                    {
                        if(cat.CategoryId == topcat.Id)
                        {
                            isParent = true;
                            break;
                        }
                    }

                    //если родительская то одно если нет то другое
                    if (isParent)
                    {
                        foreach(var chcat in Categories)
                        {
                            if(chcat.CategoryId == topcat.Id)
                            {
                                sitemapCategories.Add(chcat.EnTitle);
                            }
                        }

                        }
                    if (isParent == false)
                    {
                        sitemapCategories.Add(@topcat.EnTitle);                              
                    }
                }
            }

            return sitemapCategories;
        }

        public Category SaveCategory(Category model)
        {
            if (model.Id != 0)
            {
                var dbEntry = _ctx.Categories.SingleOrDefault(c => c.Id == model.Id);
                if (dbEntry != null)
                {
                    dbEntry.Title = model.Title;
                    dbEntry.EnTitle = model.EnTitle;
                    dbEntry.CategoryType = model.CategoryType;
                    dbEntry.CategoryId = model.CategoryId;
                    dbEntry.metaDescription = model.metaDescription;
                    dbEntry.metaKeyWords = model.metaKeyWords;

                    _ctx.Entry(dbEntry).State = EntityState.Modified;
                    _ctx.SaveChanges();
                }
            }
            else
            {
                _ctx.Categories.Add(model);
                _ctx.SaveChanges();
            }
           
            return model;
        }

        public Category GetCategoryById(int id)
        {
            return _ctx.Categories.FirstOrDefault(c => c.Id == id);
        }

        public void DeleteCategory(int itemId)
        {
            try
            {
                var item = _ctx.Categories
                            .Where(c => c.Id == itemId)
                            .Include(c=>c.ParentCategory)
                            .FirstOrDefault();
                //if (item.ParentCategory != null)
                //{
                //    item.CategoryId = null;
                //    item.ParentCategory = null;
                //    _ctx.Entry(item).State = EntityState.Modified;
                //    _ctx.SaveChanges();
                //}
                //else
                //{
                    _ctx.Entry(item).State = EntityState.Deleted;
                    _ctx.SaveChanges();
                //}
            }
            catch (Exception ex)
            {
                var tmp = 0;
            }
        }
    }
}
