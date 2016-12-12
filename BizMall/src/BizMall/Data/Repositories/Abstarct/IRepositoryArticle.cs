using System;
using System.Collections.Generic;
using System.Linq;

using BizMall.Models.CompanyModels;
using Microsoft.AspNetCore.Http;

namespace BizMall.Data.Repositories.Abstract
{
    public interface IRepositoryArticle
    {
        Article GetArticle(int goodId);
        void DeleteArticle(int goodId);
        IQueryable<Article> CompanyArticlesFullInformation(int ShopId);
        IQueryable<Article> CompanyArticles(int ShopId);
        Article SaveArticle(Article item, Company company);
    }
}
