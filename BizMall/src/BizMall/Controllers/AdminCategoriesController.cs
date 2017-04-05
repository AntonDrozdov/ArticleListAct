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
    public class AdminCategoriesController : Controller
    {
        private readonly IRepositoryArticle _repositoryArticle;
        private readonly IRepositoryCategory _repositoryCategory;
        private readonly AppSettings _settings;

        public AdminCategoriesController(IRepositoryArticle repositoryArticle,
                                        IRepositoryCategory repositoryCategory,
                                            IOptions<AppSettings> settings)
        {
            _repositoryArticle = repositoryArticle;
            _repositoryCategory = repositoryCategory;
            _settings = settings.Value;
        }

        public IActionResult Categories()
        {
            ViewBag.CategoriesVM = _repositoryCategory.Categories().ToList(); 

            ViewData["Title"] = _settings.ApplicationTitle + "Администрирование: Категории";
            ViewData["HeaderTitle"] = _settings.HeaderTitle;
            ViewData["FooterTitle"] = _settings.FooterTitle;
            ViewBag.ActiveSubMenu = "Категории";
            return View();
        }

        [HttpGet]
        public IActionResult EditCategory(int id)
        {
            ViewData["Title"] = _settings.ApplicationTitle + "Администрирование: Добавление/Редактирование категории";
            ViewData["HeaderTitle"] = _settings.HeaderTitle;
            ViewData["FooterTitle"] = _settings.FooterTitle;
            ViewBag.ActiveSubMenu = "Категории";

            if (id != 0)
                return View(_repositoryCategory.GetCategoryById(id));
            else
                return View(new Category());
        }
        [HttpPost]
        public IActionResult EditCategory(Category model)
        {

            _repositoryCategory.SaveCategory(model);

            return RedirectToAction("Categories");
        }

        /// <summary>
        /// удаление товара
        /// </summary>
        /// <param name="goodId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult DeleteCategory(int itemId)
        {
            if (itemId != 0)
            {
                _repositoryCategory.DeleteCategory(itemId);
            }
            return RedirectToAction("Categories");
        }

        [HttpPost]
        public JsonResult CategoryArticlesCount(int catId)
        {
            var category = _repositoryCategory.GetCategoryById(catId);
            PagingInfo pagingInfo;
            var Items = _repositoryArticle.CategoryArticlesFullInformation(category.EnTitle, 1, out pagingInfo).ToList();

            var CategoryArticlesCount = pagingInfo.TotalItems;
            return Json(CategoryArticlesCount);
        }
    }
}
