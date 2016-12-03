using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BizMall.Data.Repositories.Abstract;
using BizMall.ViewModels.AdminCompanyGoods;
using Microsoft.AspNetCore.Mvc;

namespace BizMall.ViewComponents
{
    public class AllCategories : ViewComponent
    {
        private readonly IRepositoryCategory _repositoryCategory;

        public AllCategories(IRepositoryCategory repositoryCategory) {
            _repositoryCategory = repositoryCategory;
        }

        //public IViewComponentResult Invoke(CreateEditGoodViewModel cegvm)
        //{
        //    ViewBag.Categories = _repositoryCategory.Categories().ToList();
        //    //string[] ws = cegvm.Category.Split('/');
        //    //ViewBag.FW = ws[0];
        //    return View(cegvm);
        //}

        public async Task<IViewComponentResult> InvokeAsync(CreateEditGoodViewModel cegvm)

        {
            ViewBag.Categories = _repositoryCategory.Categories().ToList();
            //string[] ws = cegvm.Category.Split('/');
            //ViewBag.FW = ws[0];
            return View(cegvm);
        }
    }
}
