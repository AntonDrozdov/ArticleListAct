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
    public class AdminSearchArticlesPagination : ViewComponent
    {
        public AdminSearchArticlesPagination()
        {
        
        }
        
        public async Task<IViewComponentResult> InvokeAsync(PagingInfo pi)
        {
            return View(pi);
        }
    }
}
