using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using BizMall.Models.CompanyModels;
using BizMall.Models.CommonModels;
using BizMall.Data.DBContexts;
using BizMall.Data.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using ArticleList.Models.CommonModels;
using Microsoft.Extensions.Options;

namespace BizMall.Data.Repositories.Concrete
{
    public class RepositoryArticle : RepositoryBase, IRepository, IRepositoryArticle
    {
        private readonly IRepositoryImage _repositoryImage;
        private CategoryType categoryType;
        int countOfSimilarArticlesOnArticlePage = 0;
        int pageSize = 0;
        int pageAdminSize = 0;
        int pageSearchSize = 0;

        public RepositoryArticle(ApplicationDbContext ctx, IRepositoryImage repositoryImage, IOptions<AppSettings> settings) : base(ctx)
        {
            categoryType = settings.Value.CategoryType;
            _repositoryImage = repositoryImage;

            countOfSimilarArticlesOnArticlePage = Convert.ToInt32(settings.Value.CountOfSimilarArticlesOnArticlePage);
            pageSize = Convert.ToInt32(settings.Value.PageSize);
            pageAdminSize = Convert.ToInt32(settings.Value.PageAdminSize);
            pageSearchSize = Convert.ToInt32(settings.Value.PageSearchSize);
        }

        public Article GetArticle(int goodId) {
            return _ctx.Articles
                .Where(g => g.Id == goodId)
                .Include(g => g.Category)
                .Include(g => g.Category.ParentCategory)
                .Include(g => g.Images).ThenInclude(i=>i.Image)
                .FirstOrDefault();
        }

        public IQueryable<Article> CompanyArticles(int ShopId)
        {
            //получаем список ид товаров магазина из объектов RelShopGood поля Goods, что есть связующие объекты между таблицей магазинов и таблицей товаров
            List<int> ShopGoodsIds = new List<int>();
            foreach (RelCompanyGood rsg in _ctx.Companies.Where(s => s.Id == ShopId).FirstOrDefault().Goods)
                ShopGoodsIds.Add(rsg.GoodId);

            //выбираем из таблицы товаров все, ид которых, содержаться в вышеопределенной коллекции необходимых ид
            return _ctx.Articles.Where(g => ShopGoodsIds.Contains(g.Id));
        }

        public IQueryable<Article> CompanyArticlesFullInformation(int ShopId)
        {

            //получаем список ид товаров магазина из объектов RelShopGood поля Goods, что есть связующие объекты между таблицей магазинов и таблицей товаров
            List<int> ShopGoodsIds = new List<int>();
            
            foreach (RelCompanyGood rsg in _ctx.Companies.Where(s => s.Id == ShopId).FirstOrDefault().Goods)
                ShopGoodsIds.Add(rsg.GoodId);

            //выбираем из таблицы товаров все, ид которых, содержаться в вышеопределенной коллекции необходимых ид
            List<Article> Items = new List<Article>();
            Items = _ctx.Articles
                            .Where(g => ShopGoodsIds.Contains(g.Id))
                            .Include(g => g.Category)
                            .Include(g => g.Category.ParentCategory)
                            .Include(g => g.Images).ThenInclude(g => g.Image)
                            .OrderByDescending(g=>g.UpdateTime)
                            .ToList();

            return Items.AsQueryable();
        }

        public IQueryable<Article> CompanyArticlesFullInformation(int ShopId, string Category, int page, out PagingInfo pagingInfo)
        {
            //получаем список ид товаров магазина из объектов RelShopGood поля Goods, что есть связующие объекты между таблицей магазинов и таблицей товаров
            List<int> ShopGoodsIds = new List<int>();

            foreach (RelCompanyGood rsg in _ctx.Companies.Where(s => s.Id == ShopId).FirstOrDefault().Goods)
                ShopGoodsIds.Add(rsg.GoodId);

            int totaItems = _ctx.Articles
                            .Where(g => ShopGoodsIds.Contains(g.Id)&&
                                        (Category == null || g.Category.EnTitle == Category))
                            .Count();

            pagingInfo = new PagingInfo
            {
                CategoryEnTitleForLink = Category, 
                CurrentPage = page,
                ItemsPerPage = pageAdminSize,
                TotalItems = totaItems
            };

            //выбираем из таблицы товаров все, ид которых, содержаться в вышеопределенной коллекции необходимых ид
            List<Article> Items = new List<Article>();
            Items = _ctx.Articles
                            .Where(g => ShopGoodsIds.Contains(g.Id) &&
                                        (Category == null || g.Category.EnTitle == Category))
                            .Include(g => g.Category)
                            .Include(g => g.Category.ParentCategory)
                            .OrderByDescending(g => g.UpdateTime)
                            .Skip((page - 1) * pageAdminSize)
                            .Take(pageAdminSize)
                            .ToList();


            return Items.AsQueryable();
        }

        public Article SaveArticle(Article good, Company company)
        {
            //Редактирование СУЩЕСТВУЮЩЕЙ позиции (дата UpdateStatus не меняется - она меняется только из списка неактивных товаров)
            if (good.Id != 0)
            {
                var dbEntry = _ctx.Articles.Where(g => g.Id == good.Id)
                                    .Include(g => g.Category)
                                    .Include(g => g.Category.ParentCategory)
                                    .Include(g => g.Images)
                                    .AsNoTracking()
                                    .SingleOrDefault();

                if (dbEntry != null)
                {
                    dbEntry.CategoryType = good.CategoryType;
                    dbEntry.Title = good.Title;
                    dbEntry.EnTitle = good.EnTitle;
                    dbEntry.Description = good.Description;
                    dbEntry.CategoryId = good.CategoryId;
                    dbEntry.Link = good.Link;
                    dbEntry.HashTags = good.HashTags;
                    dbEntry.UpdateTime = DateTime.Now;
                    dbEntry.metaDescription = good.metaDescription;
                    dbEntry.metaKeyWords = good.metaKeyWords;
                }

                _ctx.Entry(dbEntry).State = EntityState.Modified;
                _ctx.SaveChanges();
            }
            //Добавление НОВОЙ позиции (в т.ч. дата UpdateStatus выставляется на текущий день - берется из параметра - good)
            else
            {
                good.CategoryType = good.CategoryType;  
                _ctx.Articles.Add(good);
                _ctx.SaveChanges();

                //теперь создаем обхъкт связку товар - магазин
                RelCompanyGood rsg = new RelCompanyGood() { Good = good, GoodId = good.Id, Company = company, CompanyId = company.Id };
                //добавляем объект связку в товар
                good.Companies.Add(rsg);
                
                _ctx.Entry(good).State = EntityState.Modified;
                _ctx.SaveChanges();
            }
            return good;
        }

        private void DeleteAllArtileImages(int goodId)
        {
            Article dbEntry = _ctx.Articles.Where(g => g.Id == goodId)
                                   .Include(g => g.Category)
                                   .Include(g => g.Category.ParentCategory)
                                   .Include(g => g.Images)
                                   .SingleOrDefault();
            //какойто колхоз для получения id изображений
            List<int> imIds =  new List<int>();
            foreach (var item in dbEntry.Images) imIds.Add(item.ImageId);

            foreach (var item in imIds) _repositoryImage.DeleteImage(item);
        }

        public void DeleteArticle(int goodId)
        {
            DeleteAllArtileImages(goodId);

            Article good = _ctx.Articles.Where(g => g.Id == goodId).Include(g => g.Category).Include(g => g.Category.ParentCategory).Include(g => g.Images).FirstOrDefault();
            _ctx.Articles.Remove(good);
            _ctx.SaveChanges();
        }

        public IQueryable<Article> CategoryArticlesFullInformation(string Category, int page, out PagingInfo pagingInfo)
        {
            int totaItems = _ctx.Articles
                            .Where(g => (Category == null && g.CategoryType == categoryType) || (g.Category.EnTitle == Category && g.CategoryType == categoryType))
                            .Count();

            pagingInfo = new PagingInfo
            {
                CategoryEnTitleForLink = Category,
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalItems = totaItems
            };
            List<Article> Items = new List<Article>();
            Items = _ctx.Articles
                            .Include(g => g.Category)
                            .Include(g => g.Category.ParentCategory)
                            .Where(g => (Category == null && g.CategoryType == categoryType) || (g.Category.EnTitle == Category && g.CategoryType == categoryType))
                            .Include(g => g.Images).ThenInclude(g => g.Image)
                            .OrderByDescending(g => g.UpdateTime)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            return Items.AsQueryable();
        }

        public IQueryable<Article> CategoryArticlesFullInformation(CategoryType ct, string Category, int page, out PagingInfo pagingInfo)
        {
            int totaItems = _ctx.Articles
                            .Where(g => (Category == null && g.CategoryType == ct) || (g.Category.EnTitle == Category && g.CategoryType == ct))
                            .Count();

            pagingInfo = new PagingInfo
            {
                CategoryEnTitleForLink = Category,
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalItems = totaItems
            };
            List<Article> Items = new List<Article>();
            Items = _ctx.Articles
                            .Include(g => g.Category)
                            .Include(g => g.Category.ParentCategory)
                            .Where(g => (Category == null && g.CategoryType == ct) || (g.Category.EnTitle == Category && g.CategoryType == ct))
                            .Include(g => g.Images).ThenInclude(g => g.Image)
                            .OrderByDescending(g => g.UpdateTime)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            return Items.AsQueryable();
        }

        public IQueryable<Article> SimilarArticlesFullInformation(string Category, int ArticleId)
        {
            List<Article> Items = new List<Article>();
            Items = _ctx.Articles
                            .Include(g => g.Category)
                            .Include(g => g.Category.ParentCategory)
                            .Where(g => ((Category == null && g.CategoryType == categoryType) 
                                        || (g.Category.EnTitle == Category && g.CategoryType == categoryType))
                                        && g.Id!= ArticleId)
                            .Include(g => g.Images).ThenInclude(g => g.Image)
                            .OrderByDescending(g => g.UpdateTime)
                            .Take(countOfSimilarArticlesOnArticlePage)
                            .ToList();

            return Items.AsQueryable();
        }

        public IQueryable<Article> SearchStringArticlesFullInformation(string searchstring, int page, out PagingInfo pagingInfo)
        {
            string[] tosearch = searchstring.Split(' ');

            int seachhwordscount = tosearch.Count();
            List<int> SearchedArticlesIds = null;

            switch (seachhwordscount)
            {
                case 1:
                    SearchedArticlesIds = _ctx.Articles
                        .Where(g => (g.HashTags.Contains(tosearch[0]) || g.Description.Contains(tosearch[0]))
                                        && g.CategoryType == categoryType)
                        .Select(g => g.Id)
                        .ToList();
                        
                    break;
                case 2:
                    SearchedArticlesIds = _ctx.Articles
                        .Where(g => (g.HashTags.Contains(tosearch[0])||g.Description.Contains(tosearch[0])||g.HashTags.Contains(tosearch[1]) ||g.Description.Contains(tosearch[1]))
                                        && g.CategoryType == categoryType)
                        .Select(g => g.Id)
                        .ToList();
                    break;
                case 3:
                    SearchedArticlesIds = _ctx.Articles
                        .Where(g => (g.HashTags.Contains(tosearch[0])||g.Description.Contains(tosearch[0])||g.HashTags.Contains(tosearch[1])||g.Description.Contains(tosearch[1])||g.HashTags.Contains(tosearch[2])||g.Description.Contains(tosearch[2]))
                                         && g.CategoryType == categoryType)
                        .Select(g => g.Id)
                        .ToList();
                    break;
            }

            pagingInfo = new PagingInfo
            {
                searchstring = searchstring,
                CurrentPage = page,
                ItemsPerPage = pageSearchSize,
                TotalItems = SearchedArticlesIds.Count()
            };

            List<Article> Items = new List<Article>();
            Items = _ctx.Articles
                            .Include(g => g.Category)
                            .Include(g => g.Category.ParentCategory)
                            .Where(g => SearchedArticlesIds.Contains(g.Id))
                            .Include(g => g.Images).ThenInclude(g => g.Image)
                            .OrderByDescending(g => g.UpdateTime)
                            .Skip((page - 1) * pageSearchSize)
                            .Take(pageSearchSize)
                            .ToList();

            return Items.AsQueryable();
        }

        public IQueryable<Article> SearchStringAdminArticlesFullInformation(string searchstring, int page, out PagingInfo pagingInfo)
        {
            string[] tosearch = searchstring.Split(' ');

            int seachhwordscount = tosearch.Count();
            List<int> SearchedArticlesIds = null;

            switch (seachhwordscount)
            {
                case 1:
                    SearchedArticlesIds = _ctx.Articles
                        .Where(g => (g.HashTags.Contains(tosearch[0]) || g.Description.Contains(tosearch[0]))
                                        && g.CategoryType == categoryType)
                        .Select(g => g.Id)
                        .ToList();

                    break;
                case 2:
                    SearchedArticlesIds = _ctx.Articles
                        .Where(g => (g.HashTags.Contains(tosearch[0]) || g.Description.Contains(tosearch[0]) || g.HashTags.Contains(tosearch[1]) || g.Description.Contains(tosearch[1]))
                                        && g.CategoryType == categoryType)
                        .Select(g => g.Id)
                        .ToList();
                    break;
                case 3:
                    SearchedArticlesIds = _ctx.Articles
                        .Where(g => (g.HashTags.Contains(tosearch[0]) || g.Description.Contains(tosearch[0]) || g.HashTags.Contains(tosearch[1]) || g.Description.Contains(tosearch[1]) || g.HashTags.Contains(tosearch[2]) || g.Description.Contains(tosearch[2]))
                                         && g.CategoryType == categoryType)
                        .Select(g => g.Id)
                        .ToList();
                    break;
            }

            pagingInfo = new PagingInfo
            {
                searchstring = searchstring,
                CurrentPage = page,
                ItemsPerPage = pageAdminSize,
                TotalItems = SearchedArticlesIds.Count()
            };

            List<Article> Items = new List<Article>();
            Items = _ctx.Articles
                            .Include(g => g.Category)
                            .Include(g => g.Category.ParentCategory)
                            .Where(g => SearchedArticlesIds.Contains(g.Id))
                            .Include(g => g.Images).ThenInclude(g => g.Image)
                            .OrderByDescending(g => g.UpdateTime)
                            .Skip((page - 1) * pageAdminSize)
                            .Take(pageAdminSize)
                            .ToList();

            return Items.AsQueryable();
        }

        public IQueryable<Article> SearchHashTagArticlesFullInformation(string hashtag, int page, out PagingInfo pagingInfo)
        {
            int totaItems = _ctx.Articles
                .Where(g => g.HashTags.Contains(hashtag) 
                        && g.CategoryType == categoryType)
                .Count();

            pagingInfo = new PagingInfo
            {
                hashtag = hashtag,
                CurrentPage = page,
                ItemsPerPage = pageSearchSize,
                TotalItems = totaItems
            };
            List<Article> Items = new List<Article>();
            Items = _ctx.Articles
                            .Include(g => g.Category)
                            .Include(g => g.Category.ParentCategory)
                            .Where(g=>g.HashTags.Contains(hashtag))
                            .Include(g => g.Images).ThenInclude(g => g.Image)
                            .OrderByDescending(g => g.UpdateTime)
                            .Skip((page - 1) * pageSearchSize)
                            .Take(pageSearchSize)
                            .ToList();

            return Items.AsQueryable();
        }
    }
}

////сначала добавляем картинки в бд и тут же в коллекцию изображений товара
//foreach (IFormFile im in newimages)
//{
//    Image newim = new Image
//    {
//        Id = 0,
//        IsMain = true,
//        Description = "",
//        ImageMimeType = im.ContentType,
//        //GoodId = good.Id
//    };
                
//    using (var reader = new StreamReader(im.OpenReadStream()))
//    {
//        Stream stream = reader.BaseStream;
//        Byte[] inArray = new Byte[(int)stream.Length];
//        stream.Read(inArray, 0, (int)stream.Length);

//        newim.ImageContent = inArray;
//    }
//    //*картинки в бд
//    _repositoryImage.SaveImage(newim);

//    //*тут же в коллекцию изображений товара
//    good.Images.Clear();
//    //good.Images.Add(newim);
//}