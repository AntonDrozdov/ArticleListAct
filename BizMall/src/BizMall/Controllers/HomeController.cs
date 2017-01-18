using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BizMall.Data.Repositories.Abstract;
using BizMall.ViewModels.AdminCompanyArticles;
using BizMall.Utils;
using SimpleMvcSitemap;

namespace BizMall.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepositoryArticle _repositoryArticle;
        private readonly IRepositoryCategory _repositoryCategory;

        public HomeController(IRepositoryArticle repositoryArticle,
                              IRepositoryCategory repositoryCategory)
        {
            _repositoryArticle = repositoryArticle;
            _repositoryCategory = repositoryCategory;
        }

        public IActionResult IndexCat(string Category)
        {
            var Items = _repositoryArticle.CategoryArticlesFullInformation(Category).ToList();
            List<ArticleViewModel> ArticlesVM = new List<ArticleViewModel>();
            foreach (var item in Items)
            {
                var DescriptionLenght = (item.Description.Length > 100) ? 100 : item.Description.Length;

                ArticleViewModel avm = new ArticleViewModel
                {
                    Title = item.Title,
                    Description = item.Description,
                    UpdateTime = item.UpdateTime,
                    Category = item.Category,
                    CategoryId = item.CategoryId,
                    Link = item.Link,
                    HashTags = item.HashTags,
                    Companies = item.Companies,
                    Id = item.Id
                };

                //формируем base64 главного изображения
                if (item.Images.Count != 0)
                    avm.MainImageInBase64 = FromByteToBase64Converter.GetImageBase64Src(item.Images[0].Image);

                //формируем список изображений
                foreach (var im in item.Images)
                {
                    avm.ImagesInBase64.Add(FromByteToBase64Converter.GetImageBase64Src(im.Image));
                }

                ArticlesVM.Add(avm);
            }

            List<SitemapIndexNode> sitemapIndexNodes = new List<SitemapIndexNode>
            {
                new SitemapIndexNode(Url.Action("Categories","Sitemap")),
                new SitemapIndexNode(Url.Action("Products","Sitemap"))
            };

            var sm = new SitemapProvider(). CreateSitemapIndex(new SitemapIndexModel(sitemapIndexNodes));


            ViewBag.ArticlesVM = ArticlesVM;
            ViewBag.Category = Category;

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

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
                nodes.Add(new SitemapNode(Url.Action(sitemapcat, "Categories")));
            }

            return new SitemapProvider().CreateSitemap(new SitemapModel(nodes));
        }
    }
}
