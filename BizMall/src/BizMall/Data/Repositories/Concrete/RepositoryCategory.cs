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

        public IQueryable<Category> Categories()
        {
            return _ctx.Categories.Where(c => c.CategoryType == categoryType); 
        }

        public IQueryable<Category> ParentCategories()
        {
            return _ctx.Categories.Where(c => c.CategoryType == categoryType&&c.ParentCategory==null);
        }

        public List<string> SitemapCategories()
        {
            var Categories = _ctx.Categories.Where(c => c.CategoryType == categoryType);
            List<string> SitemapCategories = new List<string>();

            foreach(var topcat in Categories)
            {
                //определяем родительская она или нет
                if (topcat.CategoryId == null)
                {
                    bool IsParent = false;

                    foreach(var cat in Categories)
                    {
                        if(cat.CategoryId == topcat.Id)
                        {
                            IsParent = true;
                            break;
                        }
                    }

                    //если родительская то одно если нет то другое
                    if (IsParent == true)
                    {
                        foreach(var chcat in Categories)
                        {
                            if(chcat.CategoryId == topcat.Id)
                            {
                                SitemapCategories.Add(chcat.EnTitle);
                            }
                        }

                        }
                    if (IsParent == false)
                    {
                        SitemapCategories.Add(@topcat.EnTitle);                              
                    }
                }
            }

            return SitemapCategories;
        }

        public Category SaveCategory(Category model)
        {
            model.CategoryType = (model.CategoryType == CategoryType.Default) ? categoryType : model.CategoryType;

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
            return _ctx.Categories.Where(c => c.Id == id).FirstOrDefault();
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
