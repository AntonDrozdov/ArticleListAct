using BizMall.Models.CompanyModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArticleList.Models.CompanyModels
{
    public class ExternalLink
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Link { get; set; }

        public List<RelGoodExternalLink> Articles { get; set; }
        public ExternalLink()
        {
            Articles = new List<RelGoodExternalLink>();
        }
    }
}
