﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BizMall.Data.Repositories.Abstract;
using BizMall.ViewModels.AdminCompanyArticles;
using Microsoft.AspNetCore.Mvc;
using BizMall.Models.CompanyModels;

namespace BizMall.ViewComponents
{
    public class AllParentCategories : ViewComponent
    {
        private readonly IRepositoryCategory _repositoryCategory;

        public AllParentCategories(IRepositoryCategory repositoryCategory)
        {
            _repositoryCategory = repositoryCategory;
        }

        //public IViewComponentResult Invoke(CreateEditGoodViewModel cegvm)
        //{
        //    ViewBag.Categories = _repositoryCategory.Categories().ToList();
        //    //string[] ws = cegvm.Category.Split('/');
        //    //ViewBag.FW = ws[0];
        //    return View(cegvm);
        //}

        public async Task<IViewComponentResult> InvokeAsync(Category cat)

        {
            ViewBag.Categories = _repositoryCategory.ParentCategories().ToList();
            //string[] ws = cegvm.Category.Split('/');
            //ViewBag.FW = ws[0];
            return View(cat);
        }
    }
}
