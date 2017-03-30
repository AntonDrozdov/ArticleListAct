using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BizMall.Data.Repositories.Abstract;
using BizMall.ViewModels.AdminCompanyArticles;
using Microsoft.AspNetCore.Mvc;

namespace BizMall.ViewComponents
{
    public class TagCloud : ViewComponent
    {
        public TagCloud()
        {
        
        }
        
        public async Task<IViewComponentResult> InvokeAsync(ArticleViewModel avm)
        {
            //string[] ws = cegvm.Category.Split('/');

            return View(avm);
        }
    }
}
