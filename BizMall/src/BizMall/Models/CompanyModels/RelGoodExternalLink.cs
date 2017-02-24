using ArticleList.Models.CompanyModels;
using BizMall.Models.CommonModels;

namespace BizMall.Models.CompanyModels
{
    public class RelGoodExternalLink
    {
        public int ExternalLinkId { get; set; }
        public ExternalLink ExternalLink { get; set; }

        public int GoodId { get; set; }
        public Article Good { get; set; }
    }
}
