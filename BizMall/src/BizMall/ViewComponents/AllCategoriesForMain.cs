using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BizMall.Data.Repositories.Abstract;
using BizMall.ViewModels.AdminCompanyArticles;
using Microsoft.AspNetCore.Mvc;

namespace BizMall.ViewComponents
{
    public class AllCategoriesForMain: ViewComponent
    {
        private readonly IRepositoryCategory _repositoryCategory;

        public AllCategoriesForMain(IRepositoryCategory repositoryCategory)
        {
            _repositoryCategory = repositoryCategory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.Categories = _repositoryCategory.Categories().ToList();
            return View();
        }
    }
}
