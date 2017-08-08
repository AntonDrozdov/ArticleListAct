using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BizMall.Models.CompanyModels;

namespace BizMall.Data.Repositories.Abstract
{
    public interface IRepositoryCategory
    {
        Category GetCategoryByName(string entitlecategory);
        Category GetCategoryByName(string entitlecategory, CategoryType categoryType);
        Category GetCategoryById(int id);
        IQueryable<Category> ParentCategories();
        IQueryable<Category> ParentCategories(CategoryType categoryType);
        IQueryable<Category> Categories();
        IQueryable<Category> Categories(CategoryType ct);
        List<string> SitemapCategories();
        Category SaveCategory(Category model);
        void DeleteCategory(int itemId);
    }
}
