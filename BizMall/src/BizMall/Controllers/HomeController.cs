using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using BizMall.Data.Repositories.Abstract;
using BizMall.ViewModels.AdminCompanyArticles;
using BizMall.Utils;
using SimpleMvcSitemap;
using BizMall.Models.CompanyModels;
using ArticleList.Models.CommonModels;
using ArticleList.ViewModels.AdminCompanyArticles;
using BizMall.Utils.SitemapBuilder;
using Microsoft.Extensions.Options;
using SimpleMvcSitemap.Routing;

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
            List<string> hashtags = new List<string>();

            //main pages + articlepages
            nodes.Add(GetSitemapNode(null, DateTime.Now, ChangeFrequency.Daily, Convert.ToDecimal(1)));

            PagingInfo pagingInfo;

            //var Items = _repositoryArticle.CategoryArticlesFullInformation(null, 1, out pagingInfo).ToList();

            //for (int i = 0; i < pagingInfo.TotalPages; i++)
            //{
            //    nodes.Add(GetSitemapNode(string.Format("/Page{0}", i + 1), DateTime.Now));

            //    Items = _repositoryArticle.CategoryArticlesFullInformation(null, i+1, out pagingInfo).ToList();

            //    foreach (var article in Items)
            //    {
            //        List <string> articleHashTags = article.HashTags.Split(' ').ToList();
            //        hashtags.AddRange(articleHashTags);
            //        nodes.Add(GetSitemapNode("/ArticleDetails/"+ article.Id + "_" + article.EnTitle, DateTime.Now));
            //    }
            //}

            ////categorypages
            //var Categories = _repositoryCategory.Categories();
            //foreach (var category in Categories)
            //{
            //    List<Article> categorypageslist = _repositoryArticle.CategoryArticlesFullInformation(category.EnTitle, 1, out pagingInfo).ToList();
            //    if (pagingInfo.TotalPages != 0)
            //    {
            //        nodes.Add(GetSitemapNode("/Categories/" + category.EnTitle, DateTime.Now));
            //        for (var i = 0; i < pagingInfo.TotalPages; i++)
            //        {
            //            nodes.Add(GetSitemapNode(string.Format("/Categories/" + category.EnTitle + "?page={0}", i + 1), DateTime.Now));
            //        }
            //    }
            //}

            ////hashtagpager
            //hashtags = hashtags.Distinct().ToList();
            //foreach (var hashtag in hashtags)
            //{
            //    _repositoryArticle.SearchHashTagArticlesFullInformation(hashtag, 1, out pagingInfo).ToList();
            //    if (pagingInfo.TotalPages != 0)
            //    {
            //        nodes.Add(GetSitemapNode(string.Format("/Home/SearchHashTag?hashtag=" + hashtag), DateTime.Now));
            //        for (var i = 0; i < pagingInfo.TotalPages; i++)
            //        {
            //            nodes.Add(GetSitemapNode(string.Format("/Home/SearchHashTag?hashtag=" + hashtag + "&Page={0}", i + 1), DateTime.Now));
            //        }
            //    }
            //}

            ////kwpages
            var kws = _repositoryKW.KwsForSitemap().Select(kw => kw.kw).ToList();
            kws = kws.Distinct().ToList();
            foreach (var kw in kws)
            {
                var searchkw = kw.Replace(" ", "");
                _repositoryArticle.SearchHashTagArticlesFullInformation(searchkw, 1, out pagingInfo);
                if (pagingInfo.TotalPages != 0)
                {
                    nodes.Add(GetSitemapNode(string.Format("/Home/SearchKwArticles?kw=" + kw), DateTime.Now, ChangeFrequency.Weekly, Convert.ToDecimal(0.9)));
                    for (var i = 0; i < pagingInfo.TotalPages; i++)
                    {
                        nodes.Add(GetSitemapNode(string.Format("/Home/SearchKwArticles?kw=" + kw + "&Page={0}", i + 1), DateTime.Now, ChangeFrequency.Weekly, Convert.ToDecimal(0.9)));
                    }
                }
            }

            var nodecount = nodes.Count;
            var baseurlprovider = new BaseUrlProvider(){BaseUrl = new Uri("http://buisnessface.ru")}; 
            var sitemapprovider = new SitemapProvider(baseurlprovider);
            return sitemapprovider.CreateSitemap(new SitemapModel(nodes));
        }

        public IActionResult Sitemap2()
        {
            List<SitemapNode> nodes = new List<SitemapNode>();
            List<SitemapNode> statistcsNodes = new List<SitemapNode>();
            List<string> hashtags = new List<string>();
            PagingInfo pagingInfo;

            //main pages  
            nodes.Add(GetSitemapNode(null, DateTime.Now, ChangeFrequency.Daily, Convert.ToDecimal(1)));

            //#region articlepages
            //var Items = _repositoryArticle.CategoryArticlesFullInformation(null, 1, out pagingInfo).ToList();

            //for (int i = 0; i < pagingInfo.TotalPages; i++)
            //{
            //    nodes.Add(GetSitemapNode(string.Format("/Page{0}", i + 1), DateTime.Now, ChangeFrequency.Weekly, Convert.ToDecimal(0.9)));

            //    Items = _repositoryArticle.CategoryArticlesFullInformation(null, i + 1, out pagingInfo).ToList();
            //    statistcsNodes.Add(GetStatisticsSitemapNode("ArticleCount: " + Items.Count));
            //    foreach (var article in Items)
            //    {
            //        List<string> articleHashTags = article.HashTags.Split(' ').ToList();
            //        hashtags.AddRange(articleHashTags);
            //        nodes.Add(GetSitemapNode("/ArticleDetails/" + article.Id + "_" + article.EnTitle, DateTime.Now, ChangeFrequency.Weekly, Convert.ToDecimal(0.9)));
            //    }
            //}
            //#endregion

            //#region categorypages
            //var Categories = _repositoryCategory.Categories().ToList();
            //statistcsNodes.Add(GetStatisticsSitemapNode("CategoriesCount: " + Categories.Count));
            //foreach (var category in Categories)
            //{
            //    List<Article> categorypageslist = _repositoryArticle.CategoryArticlesFullInformation(category.EnTitle, 1, out pagingInfo).ToList();
            //    statistcsNodes.Add(GetStatisticsSitemapNode("Categories: " + category + "ArticleCount: " + categorypageslist.Count));
            //    if (pagingInfo.TotalPages != 0)
            //    {
            //        nodes.Add(GetSitemapNode("/Categories/" + category.EnTitle, DateTime.Now, ChangeFrequency.Weekly, Convert.ToDecimal(0.8)));
            //        for (var i = 0; i < pagingInfo.TotalPages; i++)
            //        {
            //            nodes.Add(GetSitemapNode(string.Format("/Categories/" + category.EnTitle + "?page={0}", i + 1), DateTime.Now, ChangeFrequency.Weekly, Convert.ToDecimal(0.8)));
            //        }
            //    }
            //}
            //#endregion

            //#region hashtagpager
            //hashtags = hashtags.Distinct().ToList();
            //statistcsNodes.Add(GetStatisticsSitemapNode("HashTags: " + hashtags.Count));
            //foreach (var hashtag in hashtags)
            //{
            //    _repositoryArticle.SearchHashTagArticlesFullInformation(hashtag, 1, out pagingInfo).ToList();
            //    if (pagingInfo.TotalPages != 0)
            //    {
            //        nodes.Add(GetSitemapNode(string.Format("/Home/SearchHashTag?hashtag=" + hashtag), DateTime.Now, ChangeFrequency.Weekly, Convert.ToDecimal(0.7)));
            //        for (var i = 0; i < pagingInfo.TotalPages; i++)
            //        {
            //            nodes.Add(GetSitemapNode(string.Format("/Home/SearchHashTag?hashtag=" + hashtag + "&Page={0}", i + 1), DateTime.Now, ChangeFrequency.Weekly, Convert.ToDecimal(0.7)));
            //        }
            //    }
            //}
            //#endregion

            #region kwpages
            var kws = _repositoryKW.KwsForSitemap().Select(kw => kw.kw).ToList();
            kws = kws.Distinct().ToList();
            statistcsNodes.Add(GetStatisticsSitemapNode("KWS: " + kws.Count));
            foreach (var kw in kws)
            {
                var searchkw = kw.Replace(" ", "");
                _repositoryArticle.SearchHashTagArticlesFullInformation(searchkw, 1, out pagingInfo);
                if (pagingInfo.TotalPages != 0)
                {
                    nodes.Add(GetSitemapNode(string.Format("/Home/SearchKwArticles?kw=" + kw), DateTime.Now, ChangeFrequency.Weekly, Convert.ToDecimal(0.7)));
                    for (var i = 0; i < pagingInfo.TotalPages; i++)
                    {
                        nodes.Add(GetSitemapNode(string.Format("/Home/SearchKwArticles?kw=" + kw + "&Page={0}", i + 1), DateTime.Now, ChangeFrequency.Weekly, Convert.ToDecimal(0.7)));
                    }
                }
            }
            #endregion

            string baseUrl = _settings.DomainName;
            var siteMapBuilder = new SitemapBuilder();

            //add statistics data to sitemap
            foreach (var statisticNode in statistcsNodes)
            {
                siteMapBuilder.AddUrl(baseUrl + statisticNode.Url, statisticNode.LastModificationDate, statisticNode.ChangeFrequency, statisticNode.Priority);
            }

            // add sitepages to the sitemap
            foreach (var node in nodes)
            {
                siteMapBuilder.AddUrl(baseUrl + node.Url, node.LastModificationDate, node.ChangeFrequency , node.Priority);
            }

            // generate the sitemap xml


            var path = "./wwwroot/sitemap.xml";
            XDocument xdoc = siteMapBuilder.GetSitemap();

            FileStream fileStream = new FileStream(path, FileMode.Create);
            XmlWriterSettings settings = new XmlWriterSettings() { Indent = true };
            XmlWriter writer = XmlWriter.Create(fileStream, settings);

            xdoc.Save(writer);
            writer.Flush();
            fileStream.Flush();


            byte[] fileBytes = System.Text.Encoding.Unicode.GetBytes(xdoc.ToString());

            //byte[] fileBytes = xdoc.ToString(). System.IO.File.ReadAllBytes(path);
            return File(fileBytes, "application/x-msdownload", _settings.DomainName + "_sitemap.xml");
            //string xml = siteMapBuilder.ToString();
            //return Content(xml, "text/xml");
        }

        [HttpPost]
        public JsonResult KwsForTagCloud(int CategoryId)
        {
            var categoryKws = _repositoryKW.KwsForTagCloud(CategoryId).ToList();
            return Json(categoryKws);
        }


        private SitemapNode GetSitemapNode(string url, DateTime lastModified, ChangeFrequency changeFrequency, decimal priority)
        {
            var currentSitemapNode = new SitemapNode(url);
            currentSitemapNode.LastModificationDate = lastModified;
            currentSitemapNode.ChangeFrequency = changeFrequency;
            currentSitemapNode.Priority = priority;

            return currentSitemapNode;
        }

        private SitemapNode GetStatisticsSitemapNode(string data)
        {
            return new SitemapNode(data);
        }
    }

    public class BaseUrlProvider:IBaseUrlProvider
    {
        public Uri BaseUrl { get; set; }
    }
}
