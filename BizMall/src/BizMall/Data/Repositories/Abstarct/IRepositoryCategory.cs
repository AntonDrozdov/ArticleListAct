﻿using System;
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
        IQueryable<Category> ParentCategories(CategoryType curCategoryType);
        IQueryable<Category> Categories(CategoryType ct);
        IQueryable<Category> Categories();
        List<string> SitemapCategories();
        Category SaveCategory(Category model);
        void DeleteCategory(int itemId);
    }
}
