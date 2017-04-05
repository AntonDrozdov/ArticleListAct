using System;
using System.Collections.Generic;
using System.Linq;

using BizMall.Data.Repositories.Abstract;
using BizMall.ViewModels.AdminCompanyArticles;
using BizMall.Models.CompanyModels;
using BizMall.Models.CommonModels;
using BizMall.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using ArticleList.Models.CommonModels;
using Microsoft.Extensions.Options;

namespace BizMall.Controllers
{
    /// <summary>
    /// ctor
    /// </summary>
    [Authorize]
    public class AdminKWController : Controller
    {
        private readonly IRepositoryArticle _repositoryArticle;
        private readonly IRepositoryCategory _repositoryCategory;
        private readonly IRepositoryKW _repositoryKW;
        private readonly AppSettings _settings;

        public AdminKWController(IRepositoryArticle repositoryArticle,
                                IRepositoryKW repositoryKW, 
                                IRepositoryCategory repositoryCategory,
                                IOptions<AppSettings> settings)
        {
            _repositoryArticle = repositoryArticle;
            _repositoryKW = repositoryKW;
            _repositoryCategory = repositoryCategory;
            _settings = settings.Value;
        }

        public IActionResult Kws(string Category, int Page = 1)
        {
            PagingInfo pagingInfo;
            var Items = _repositoryKW.Kws(Category, Page, out pagingInfo).ToList();

            ViewData["Title"] = _settings.ApplicationTitle + "Администрирование: Ключевики";
            ViewData["HeaderTitle"] = _settings.HeaderTitle;
            ViewData["FooterTitle"] = _settings.FooterTitle;
            ViewBag.KWs = Items;
            ViewBag.PagingInfo = pagingInfo;
            ViewBag.ActiveSubMenu = "Ключевики";

            return View();
        }

        public IActionResult Search(string searchstring, int Page = 1)
        {
            if (searchstring == null)
                return RedirectToAction("Kws");


            PagingInfo pagingInfo;
            var Items = _repositoryKW.SearchStringKws(searchstring, Page, out pagingInfo).ToList();

            ViewData["Title"] = _settings.ApplicationTitle + "Поиск: " + searchstring;
            ViewData["HeaderTitle"] = _settings.HeaderTitle;
            ViewData["FooterTitle"] = _settings.FooterTitle;
            ViewBag.KWs = Items;
            ViewBag.PagingInfo = pagingInfo;
            ViewBag.ActiveSubMenu = "Ключевики";

            return View();
        }


        [HttpGet]
        public IActionResult EditKw(int id)
        {
            ViewData["Title"] = _settings.ApplicationTitle + "Администрирование: Добавление/Редактирование ключевика";
            ViewData["HeaderTitle"] = _settings.HeaderTitle;
            ViewData["FooterTitle"] = _settings.FooterTitle;
            ViewBag.ActiveSubMenu = "Ключевики";

            if (id != 0)
            {
                var item = _repositoryKW.GetKwById(id);
                return View(new CreateEditKWViewModel
                {
                    Id = item.Id,
                    kw = item.kw,
                    CategoryId = (item.CategoryId!=0)?item.CategoryId:0,
                    CategoryTitle = (item.Category!=null)?item.Category.Title:null,
                    CategoryType = item.CategoryType
                });
            }
            else
                return View(new CreateEditKWViewModel());
        }
        [HttpPost]
        public IActionResult EditKw(CreateEditKWViewModel model)
        {
            _repositoryKW.SaveKW(new KW
            {
                Id = model.Id,
                kw = model.kw,
                CategoryId = (model.CategoryId != 0) ? model.CategoryId : 0
            });

            return RedirectToAction("Kws");
        }

        [HttpPost]
        public JsonResult KwArticlesCount(int kwid)
        {
            var kw = _repositoryKW.GetKwById(kwid);
            PagingInfo pagingInfo;
            var searchkw = kw.kw.Replace(" ", "");
            var Items = _repositoryArticle.SearchHashTagArticlesFullInformation(searchkw, 1, out pagingInfo).ToList();

            var KwArticlesCount = pagingInfo.TotalItems;
            return Json(KwArticlesCount);
        }

        /// <summary>
        /// удаление товара
        /// </summary>
        /// <param name="goodId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult DeleteKw(int itemId)
        {
            if (itemId != 0)
            {
                _repositoryKW.DeleteKW(itemId);
            }
            return RedirectToAction("Kws");
        }
    }
}
