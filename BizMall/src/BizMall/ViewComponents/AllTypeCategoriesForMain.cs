using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BizMall.Data.Repositories.Abstract;
using BizMall.ViewModels.AdminCompanyArticles;
using Microsoft.AspNetCore.Mvc;
using BizMall.Models.CompanyModels;

namespace BizMall.ViewComponents
{
    public class AllTypeCategoriesForMain: ViewComponent
    {
        private readonly IRepositoryCategory _repositoryCategory;

        public AllTypeCategoriesForMain(IRepositoryCategory repositoryCategory)
        {
            _repositoryCategory = repositoryCategory;
        }

        public async Task<IViewComponentResult> InvokeAsync(CategoryType ct)
        {
            ViewBag.Categories = _repositoryCategory.Categories(ct).ToList();
            ViewBag.CategoryType = ct;
            //string[] ws = cegvm.Category.Split('/');
            //ViewBag.FW = ws[0];
            return View(ct);
        }
    }
}
