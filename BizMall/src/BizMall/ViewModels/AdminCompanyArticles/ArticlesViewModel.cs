using ArticleList.Models.CommonModels;
using BizMall.ViewModels.AdminCompanyArticles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArticleList.ViewModels.AdminCompanyArticles
{
    public class ArticlesViewModel
    {
        public List<ArticleViewModel> articlesViewModels { get; set; }
        public PagingInfo pagingInfo { get; set; }

        public ArticlesViewModel()
        {
            articlesViewModels = new List<ArticleViewModel>();
        }
    }
}
