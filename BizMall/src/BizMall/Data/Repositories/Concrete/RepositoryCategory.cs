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
            //return _ctx.Categories.Where(c => c.CategoryType == categoryType);
            return _ctx.Categories;
        }

        public IQueryable<Category> Categories(CategoryType ct)
        {
            return _ctx.Categories.Where(c => c.CategoryType == ct);
        }

        public IQueryable<Category> ParentCategories(CategoryType categoryType)
        {
            return _ctx.Categories.Where(c => c.CategoryType == categoryType && c.ParentCategory == null);
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
            //Редактирование СУЩЕСТВУЮЩЕЙ позиции (дата UpdateStatus не меняется - она меняется только из списка неактивных товаров)
            if (model.Id != 0)
            {
                var dbEntry = _ctx.Categories.Where(c => c.Id == model.Id).SingleOrDefault();
                if (dbEntry != null)
                {
                    if (model.ParentCategory.Id!=0)
                    {
                        dbEntry.CategoryType = model.CategoryType;
                        dbEntry.Title = model.Title;
                        dbEntry.EnTitle = model.EnTitle;
                        dbEntry.CategoryId = model.ParentCategory.Id;
                        dbEntry.ParentCategory = null;
                        dbEntry.metaDescription = model.metaDescription;
                        dbEntry.metaKeyWords = model.metaKeyWords;
                    }
                    else
                    {
                        dbEntry.CategoryType = model.CategoryType;
                        dbEntry.Title = model.Title;
                        dbEntry.EnTitle = model.EnTitle;
                        dbEntry.CategoryId = null;
                        dbEntry.ParentCategory = null;
                        dbEntry.metaDescription = model.metaDescription;
                        dbEntry.metaKeyWords = model.metaKeyWords;
                    }
                }
                _ctx.Entry(dbEntry).State = EntityState.Modified;
                _ctx.SaveChanges();
            }
            //Добавление НОВОЙ позиции (в т.ч. дата UpdateStatus выставляется на текущий день - берется из параметра - good)
            else
            {
                if (model.ParentCategory.Id <= 0)
                {
                    model.ParentCategory = null;
                }
                else
                {
                    model.CategoryId = model.ParentCategory.Id;
                    model.ParentCategory = null;
                }
                model.CategoryType = model.CategoryType;
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
