using System;
using System.Collections.Generic;
using System.Linq;

using BizMall.Models.CompanyModels;
using Microsoft.AspNetCore.Http;
using ArticleList.Models.CommonModels;

namespace BizMall.Data.Repositories.Abstract
{
    public interface IRepositoryArticle
    {
        Article GetArticle(int goodId);
        void DeleteArticle(int goodId);
        IQueryable<Article> CompanyArticlesFullInformation(int ShopId);
        IQueryable<Article> CompanyArticlesFullInformation(int ShopId, string Category, int page, out PagingInfo pagingInfo);
        IQueryable<Article> CategoryArticlesFullInformation(string Category, int page, out PagingInfo pagingInfo);
        IQueryable<Article> SimilarArticlesFullInformation(string Category, int ArticleId);
        
        IQueryable<Article> SearchStringArticlesFullInformation(string searchstring, int page, out PagingInfo pagingInfo);
        IQueryable<Article> SearchStringAdminArticlesFullInformation(string searchstring, int page, out PagingInfo pagingInfo);
        IQueryable<Article> SearchHashTagArticlesFullInformation(string hashtag, int page, out PagingInfo pagingInfo);

        IQueryable<Article> CompanyArticles(int ShopId);
        Article SaveArticle(Article item, Company company);
    }
}
