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

        public string Description { get; set; }

        public int? CategoryId { get; set; }
        public Category Category { get; set; }

        public DateTime UpdateTime { get; set; }

        public List<RelCompanyGood> Companies { get; set; }

        public List<RelGoodImage> Images { get; set; }
        
        public Article()
        {
            Images = new List<RelGoodImage>();
            //Discounts = new List<RelDiscountGood>();
            Companies = new List<RelCompanyGood>();
        }
    }
}