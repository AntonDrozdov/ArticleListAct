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
            CreateEditCategoryViewModel cecvm = (id != 0) ? 
                ConstructCECVM(_repositoryCategory.GetCategoryById(id)) : 
                new CreateEditCategoryViewModel(); 

            ViewData["Title"] = _settings.ApplicationTitle + "Администрирование: Добавление/Редактирование категории";
            ViewData["HeaderTitle"] = _settings.HeaderTitle;
            ViewData["FooterTitle"] = _settings.FooterTitle;
            ViewBag.ActiveSubMenu = "Категории";

            return View(cecvm);
        }
        [HttpPost]
        public IActionResult EditCategory(CreateEditCategoryViewModel model)
        {
            SaveCategoryChanges(model);

            return RedirectToAction("Categories");
        }

        /// <summary>
        /// удаление категории
        /// </summary>
        [HttpGet]
        public IActionResult DeleteCategory(int itemId)
        {
            if (itemId != 0)
            {
                _repositoryCategory.DeleteCategory(itemId);
            }
            return RedirectToAction("Categories");
        }

        /// <summary>
        /// считаем сколько статей в категории
        /// </summary>
        [HttpPost]
        public JsonResult CategoryArticlesCount(int catId)
        {
            var category = _repositoryCategory.GetCategoryById(catId);
            PagingInfo pagingInfo;
            var Items = _repositoryArticle.CategoryArticlesFullInformation(category.EnTitle, 1, out pagingInfo).ToList();

            var CategoryArticlesCount = pagingInfo.TotalItems;
            return Json(CategoryArticlesCount);
        }

        #region PRIVATE METHODS

        private CreateEditCategoryViewModel ConstructCECVM(Category item)
        {
            return new CreateEditCategoryViewModel
            {
                Id =item.Id,
                Title = item.Title,
                EnTitle = item.EnTitle,
                CategoryType = item.CategoryType,
                ParentCategoryId = item.CategoryId,
                ParentCategoryTitle = item.ParentCategory.Title,                
                metaDescription = item.metaDescription,
                metaKeyWords = item.metaKeyWords
            };
        }

        public void SaveCategoryChanges(CreateEditCategoryViewModel model)
        {
            _repositoryCategory.SaveCategory(new Category
            {
                Id = model.Id,
                Title = model.Title,
                EnTitle = model.EnTitle,
                CategoryType = model.CategoryType,
                CategoryId = model.ParentCategoryId,
                metaDescription = model.metaDescription,
                metaKeyWords = model.metaKeyWords
            });
        }
        

        #endregion
    }
}
