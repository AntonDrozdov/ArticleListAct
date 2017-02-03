using System.ComponentModel.DataAnnotations;
using BizMall.Models.CommonModels;
using System.Collections.Generic;
using BizMall.Models.CompanyModels;
using ArticleList.Models.CommonModels;
using System;

namespace BizMall.ViewModels.AdminCompanyArticles
{
    public class ArticleViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string EnTitle { get; set; }

        public string Description { get; set; }

        public List<string> HashTags { get; set; }

        public string Link { get; set; }

        public int? CategoryId { get; set; }
        public Category Category { get; set; }

        public DateTime UpdateTime { get; set; }

        public List<string> ImagesInBase64 { get; set; }

        public ICollection<Image> Images { get; set; }
        public List<RelCompanyGood> Companies { get; set; }

        public ArticleViewModel()
        {
            Images = new List<Image>();
            Companies = new List<RelCompanyGood>();
            ImagesInBase64 = new List<string>();
            HashTags = new List<string>();
        }

        public string MainImageInBase64 { get; set; }

        
    }
}
