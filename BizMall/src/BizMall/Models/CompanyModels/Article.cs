using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BizMall.Models.CommonModels;
using System;

namespace BizMall.Models.CompanyModels
{
    public class Article
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string EnTitle { get; set; }

        public string Description { get; set; }

        public string HashTags { get; set; }

        public string Link { get; set; }

        public CategoryType CategoryType { get; set; }

        public int? CategoryId { get; set; }
        public Category Category { get; set; }

        public DateTime UpdateTime { get; set; }

        public List<RelGoodImage> Images { get; set; }
        public List<RelCompanyGood> Companies { get; set; }
        public List<RelGoodExternalLink> ExternalLinks { get; set; }

        public Article()
        {
            Images = new List<RelGoodImage>();
            Companies = new List<RelCompanyGood>();
            ExternalLinks = new List<RelGoodExternalLink>();
    }
    }
}