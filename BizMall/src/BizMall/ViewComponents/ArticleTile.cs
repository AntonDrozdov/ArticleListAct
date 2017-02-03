using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BizMall.Data.Repositories.Abstract;
using BizMall.ViewModels.AdminCompanyArticles;
using Microsoft.AspNetCore.Mvc;

namespace BizMall.ViewComponents
{
    public class ArticleTile: ViewComponent
    {
        public ArticleTile()
        {
        
        }
        
        public async Task<IViewComponentResult> InvokeAsync(ArticleViewModel avm)
        {
            //string[] ws = cegvm.Category.Split('/');
            //ViewBag.FW = ws[0];
            return View(avm);
        }
    }
}
