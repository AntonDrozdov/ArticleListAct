﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BizMall.Data.Repositories.Abstract;
using BizMall.ViewModels.AdminCompanyArticles;
using BizMall.Utils;
using SimpleMvcSitemap;
using BizMall.Models.CompanyModels;
using ArticleList.Models.CommonModels;
using ArticleList.ViewModels.AdminCompanyArticles;
using Microsoft.Extensions.Options;

namespace BizMall.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepositoryArticle _repositoryArticle;
        private readonly IRepositoryCategory _repositoryCategory;
        private readonly IRepositoryKW _repositoryKW;
        private readonly AppSettings _settings;

        public HomeController(  IRepositoryArticle repositoryArticle,
                                IRepositoryCategory repositoryCategory,
                                IRepositoryKW repositoryKW,
                                IOptions<AppSettings> settings)
        {
            _repositoryArticle = repositoryArticle;
            _repositoryCategory = repositoryCategory;
            _repositoryKW = repositoryKW;
            _settings = settings.Value;
        }

        private ArticleViewModel ConstructAVM(Article item, bool shortDescription)
        {
            string actualDecription = (shortDescription && item.Description.Length > 300) ? item.Description.Substring(0, 300) + " . . ." : item.Description;
            //actualDecription = actualDecription.Replace("[newstr]", "</p><p>");
            ArticleViewModel avm = new ArticleViewModel
            {
                Title = item.Title,
                EnTitle = item.EnTitle,
                Description = actualDecription,
                UpdateTime = item.UpdateTime,
                Category = item.Category,
                CategoryId = item.CategoryId,
                Link = item.Link,
                Companies = item.Companies,
                Id = item.Id
            };
            string[] hashtags = item.HashTags.Split(' ');
            foreach (var hashtag in hashtags) avm.HashTags.Add(hashtag);

            //формируем список изображений
            foreach (var im in item.Images)
            {
                avm.ImagesInBase64.Add(FromByteToBase64Converter.GetImageBase64Src(im.Image));
            }

            return avm;
        }

        //Главные представления
        public IActionResult IndexCat(string Category, int Page = 1)
        {
            PagingInfo pagingInfo;
            var Items = _repositoryArticle.CategoryArticlesFullInformation(Category, Page, out pagingInfo).ToList();
            
            List<ArticleViewModel> ArticlesVM = new List<ArticleViewModel>();
            foreach (var item in Items)
            {
                var avm = ConstructAVM(item, true); 
                ArticlesVM.Add(avm);
            }

            if (Category != null)
            {
                var category = _repositoryCategory.GetCategoryByName(Category);

                ViewData["Title"] = _settings.ApplicationTitle + category.Title;
                ViewBag.Category = category.Title;
                ViewBag.CategoryId = category.Id;
                ViewData["metaDescription"] = "";
                ViewData["metaKeyWords"] = "";


            }
            else
            {
                ViewData["Title"] = _settings.ApplicationTitle + "Главная";
                ViewBag.CategoryId = 0;
                ViewData["metaDescription"] = "";
                ViewData["metaKeyWords"] = "";

            }

            ViewBag.Kws = _repositoryKW.KwsForTagCloud(ViewBag.CategoryId);
            ViewData["HeaderTitle"] = _settings.HeaderTitle;
            ViewData["FooterTitle"] = _settings.FooterTitle;

            ViewBag.ArticlesVM = ArticlesVM;
            ViewBag.PagingInfo = pagingInfo;
            

            return View();
        }

        public IActionResult Search(string searchstring, int Page = 1)
        {
            if (searchstring == null)
                return RedirectToAction("IndexCat");

            PagingInfo pagingInfo;
            var Items = _repositoryArticle.SearchStringArticlesFullInformation(searchstring, Page, out pagingInfo).ToList();
        
            List<ArticleViewModel> ArticlesVM = new List<ArticleViewModel>();
            foreach (var item in Items)
            {
                var avm = ConstructAVM(item, true);
                ArticlesVM.Add(avm);                
            }

            ViewData["Title"] = _settings.ApplicationTitle + "Поиск: " + searchstring;
            ViewData["HeaderTitle"] = _settings.HeaderTitle;
            ViewData["FooterTitle"] = _settings.FooterTitle;
            ViewData["metaDescription"] = "Результаты поиска по: " + searchstring;
            ViewData["metaKeyWords"] = searchstring;
            ViewBag.ArticlesVM = ArticlesVM;
            ViewBag.PagingInfo = pagingInfo;
            ViewBag.PageTitle = "Результаты поиска по: " + searchstring;
            ViewBag.Kws = _repositoryKW.KwsForTagCloud(0);

            return View();
        }

        public IActionResult SearchHashTag(string hashtag, int Page = 1)
        {
            PagingInfo pagingInfo;
            var Items = _repositoryArticle.SearchHashTagArticlesFullInformation(hashtag, Page, out pagingInfo).ToList();

            List<ArticleViewModel> ArticlesVM = new List<ArticleViewModel>();
            foreach (var item in Items)
            {
                var avm = ConstructAVM(item, true);
                ArticlesVM.Add(avm);
            }

            ViewData["Title"] = _settings.ApplicationTitle + "Хэштег: " + hashtag;
            ViewData["HeaderTitle"] = _settings.HeaderTitle;
            ViewData["FooterTitle"] = _settings.FooterTitle;
            ViewData["metaDescription"] = "По хэштегу: #" + hashtag;
            ViewData["metaKeyWords"] = hashtag;
            ViewBag.ArticlesVM = ArticlesVM;
            ViewBag.PagingInfo = pagingInfo;
            ViewBag.PageTitle = "По хэштегу: #" + hashtag;
            ViewBag.Kws = _repositoryKW.KwsForTagCloud(0);

            return View();
        }

        public IActionResult SearchKwArticles(string kw, int Page = 1)
        {
            PagingInfo pagingInfo;
            var searchkw = kw.Replace(" ", "");
            var Items = _repositoryArticle.SearchHashTagArticlesFullInformation(searchkw, Page, out pagingInfo).ToList();

            List<ArticleViewModel> ArticlesVM = new List<ArticleViewModel>();
            foreach (var item in Items)
            {
                var avm = ConstructAVM(item, true);
                ArticlesVM.Add(avm);
            }

            ViewData["Title"] = _settings.ApplicationTitle + "По запросу: " + kw;
            ViewData["HeaderTitle"] = _settings.HeaderTitle;
            ViewData["FooterTitle"] = _settings.FooterTitle;
            ViewData["metaDescription"] = "По запросу: " + kw;
            ViewData["metaKeyWords"] = kw;
            ViewBag.ArticlesVM = ArticlesVM;
            ViewBag.PagingInfo = pagingInfo;
            ViewBag.PageTitle = "По запросу: " + kw;
            ViewBag.Kws = _repositoryKW.KwsForTagCloud(0);

            return View();
        }

        public IActionResult ArticleDetails(string articleId)
        {
            int Id = Convert.ToInt32(articleId.Substring(0, articleId.IndexOf('_')));
            var item = _repositoryArticle.GetArticle(Id);

            if (item == null)
                return Error();

            var avm = ConstructAVM(item, false);

            ViewData["Title"] = _settings.ApplicationTitle + avm.Title;
            ViewData["HeaderTitle"] = _settings.HeaderTitle;
            ViewData["FooterTitle"] = _settings.FooterTitle;
            ViewData["metaDescription"] = item.metaDescription;
            ViewData["metaKeyWords"] = item.metaKeyWords;

            ViewBag.ArticleVM = avm;

            //похожие статьи (для того чтобы наполнить страницу нужным количеством символов)
            var SimilarArticlesVM = new List<ArticleViewModel>();
            if (Convert.ToInt32(_settings.CountOfSimilarArticlesOnArticlePage) > 0)
            {
                var Items = _repositoryArticle.SimilarArticlesFullInformation(item.Category.EnTitle, item.Id).ToList();

                foreach (var i in Items)
                {
                    if (i.Id != avm.Id)
                        SimilarArticlesVM.Add(ConstructAVM(i, true));
                }
            }

            ViewBag.Kws = _repositoryKW.KwsForTagCloud(Convert.ToInt32(item.CategoryId));
            ViewBag.SimilarArticlesVM = SimilarArticlesVM;           

            return View();
        }

        public IActionResult About()
        {
            ViewData["Title"] = _settings.ApplicationTitle + "О проекте";
            ViewData["HeaderTitle"] = _settings.HeaderTitle;
            ViewData["FooterTitle"] = _settings.FooterTitle;
            ViewData["metaDescription"] = _settings.metaDescription;
            ViewData["metaKeyWords"] = _settings.metaKeyWords;

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Sitemap()
        {
            List<SitemapNode> nodes = new List<SitemapNode>();

            //main pages + articlepages
            nodes.Add(GetSitemapNode(null, DateTime.Now));

            PagingInfo pagingInfo;

            var Items = _repositoryArticle.CategoryArticlesFullInformation(null, 1, out pagingInfo).ToList();

            for (int i = 0; i < pagingInfo.TotalPages; i++)
            {
                nodes.Add(GetSitemapNode(string.Format("/Page{0}", i + 1), DateTime.Now));

                Items = _repositoryArticle.CategoryArticlesFullInformation(null, i+1, out pagingInfo).ToList();

                foreach (var article in Items)
                {
                    nodes.Add(GetSitemapNode("/ArticleDetails/"+ article.Id + "_" + article.EnTitle, DateTime.Now));
                }
            }

            //categorypages
            var Categories = _repositoryCategory.Categories();
            foreach (var catogory in Categories)
            {
                nodes.Add(GetSitemapNode("/Categories/" + catogory.EnTitle, DateTime.Now));

                List<Article> list = new List<Article>();
                foreach (var article in _repositoryArticle.CategoryArticlesFullInformation(null, 1, out pagingInfo))
                    list.Add(article);
         
                for (var i = 0; i < pagingInfo.TotalPages; i++)
                {
                    nodes.Add(GetSitemapNode(string.Format("/Categories/" + catogory.EnTitle + "?page={0}", i+1), DateTime.Now));
                }
            }

            //hashtagpager

            //kwpages

            var nodecount = nodes.Count;
            return new SitemapProvider().CreateSitemap(new SitemapModel(nodes));
        }

        [HttpPost]
        public JsonResult KwsForTagCloud(int CategoryId)
        {
            var categoryKws = _repositoryKW.KwsForTagCloud(CategoryId).ToList();
            return Json(categoryKws);
        }


        private SitemapNode GetSitemapNode(string url, DateTime lastModified)
        {
            var currentSitemapNode = new SitemapNode(url);
            currentSitemapNode.LastModificationDate = lastModified;

            return currentSitemapNode;
        }
    }
}
