using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BizMall.Data.Repositories.Abstract;
using BizMall.ViewModels.AdminCompanyArticles;
using Microsoft.AspNetCore.Mvc;
using ArticleList.Models.CommonModels;

namespace BizMall.ViewComponents
{
    public class CategoryPagination : ViewComponent
    {
        public CategoryPagination()
        {
        
        }
        
        public async Task<IViewComponentResult> InvokeAsync(PagingInfo pi)
        {
            //string[] ws = cegvm.Category.Split('/');
            //ViewBag.FW = ws[0];
            return View(pi);
        }
    }
}
