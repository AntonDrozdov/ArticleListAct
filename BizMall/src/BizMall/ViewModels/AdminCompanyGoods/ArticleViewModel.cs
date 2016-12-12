using System.ComponentModel.DataAnnotations;
using BizMall.Models.CommonModels;
using System.Collections.Generic;
using BizMall.Models.CompanyModels;
using System;

namespace BizMall.ViewModels.AdminCompanyGoods
{
    public class ArticleViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string HashTags { get; set; }

        public string Link { get; set; }

        public int? CategoryId { get; set; }
        public Category Category { get; set; }

        public DateTime UpdateTime { get; set; }

        public ICollection<Image> Images { get; set; }
        public List<RelCompanyGood> Companies { get; set; }

        public ArticleViewModel()
        {
            Images = new List<Image>();
            Companies = new List<RelCompanyGood>();
        }

        public string MainImageInBase64 { get; set; }
    }
}
