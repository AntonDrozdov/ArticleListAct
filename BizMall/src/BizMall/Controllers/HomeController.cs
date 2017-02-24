using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly AppSettings _settings;

        public HomeController(IRepositoryArticle repositoryArticle,
                              IRepositoryCategory repositoryCategory,
                              IOptions<AppSettings> settings)
        {
            _repositoryArticle = repositoryArticle;
            _repositoryCategory = repositoryCategory;
            _settings = settings.Value;
        }

        private ArticleViewModel ConstructAVM(Article item, bool shortDescription)
        {
            ArticleViewModel avm = new ArticleViewModel
            {
                Title = item.Title,
                EnTitle = item.EnTitle,
                Description = (shortDescription && item.Description.Length > 300) ? item.Description.Substring(0, 300) + " . . ." : item.Description,
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
                if (Items.Count > 0)
                {
                    ViewData["Title"] = _settings.ApplicationTitle + Items[0].Category.Title;
                    ViewBag.Category = Items[0].Category.Title;
                }
                else
                {
                    ViewData["Title"] = _settings.ApplicationTitle + _repositoryCategory.GetCategoryByName(Category).Title; 
                }
            }
            else
                ViewData["Title"] = _settings.ApplicationTitle + "Главная";

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
            ViewBag.ArticlesVM = ArticlesVM;
            ViewBag.PagingInfo = pagingInfo;

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
            ViewBag.ArticlesVM = ArticlesVM;
            ViewBag.PagingInfo = pagingInfo;

            return View();
        }

        public IActionResult ArticleDetails(string articleId)
        {
            int Id = Convert.ToInt32(articleId.Substring(0, articleId.IndexOf('_')));
            var item = _repositoryArticle.GetArticle(Id);            

            var avm = ConstructAVM(item, false);

            ViewData["Title"] = _settings.ApplicationTitle + avm.Title;
            ViewData["HeaderTitle"] = _settings.HeaderTitle;
            ViewData["FooterTitle"] = _settings.FooterTitle;
            ViewBag.ArticleVM = avm;
            
            return View();
        }

        public IActionResult About()
        {
            ViewData["Title"] = _settings.ApplicationTitle + "О проекте";
            ViewData["HeaderTitle"] = _settings.HeaderTitle;
            ViewData["FooterTitle"] = _settings.FooterTitle;

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Sitemap()
        {
            List<string> SitemapCategories = _repositoryCategory.SitemapCategories();

            List<SitemapNode> nodes = new List<SitemapNode>();
            foreach (var sitemapcat in SitemapCategories)
            {
                nodes.Add(new SitemapNode("/Categories/" + sitemapcat));
            }

            return new SitemapProvider().CreateSitemap(new SitemapModel(nodes));
        }


    }
}
