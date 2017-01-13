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

namespace BizMall.Data.Repositories.Concrete
{
    public class RepositoryArticle : RepositoryBase, IRepository, IRepositoryArticle
    {
        private readonly IRepositoryImage _repositoryImage;

        public RepositoryArticle(ApplicationDbContext ctx, IRepositoryImage repositoryImage) : base(ctx)
        {
            _repositoryImage = repositoryImage;
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
                            .ToList();

            return Items.AsQueryable();
        }

        public IQueryable<Article> TopArticlesFullInformation()
        {
            //выбираем из таблицы товаров все, ид которых, содержаться в вышеопределенной коллекции необходимых ид
            List<Article> Items = new List<Article>();
            Items = _ctx.Articles
                            .Include(g => g.Category)
                            .Include(g => g.Category.ParentCategory)
                            .Include(g => g.Images).ThenInclude(g => g.Image)
                            .OrderByDescending(g => g.UpdateTime)
                            .Take(10)
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
                    dbEntry.Title = good.Title;
                    dbEntry.Description = good.Description;
                    dbEntry.CategoryId = good.CategoryId;
                    dbEntry.Link = good.Link;
                    dbEntry.HashTags = good.HashTags;
                }

                _ctx.Entry(dbEntry).State = EntityState.Modified;
                _ctx.SaveChanges();
            }
            //Добавление НОВОЙ позиции (в т.ч. дата UpdateStatus выставляется на текущий день - берется из параметра - good)
            else
            {         
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