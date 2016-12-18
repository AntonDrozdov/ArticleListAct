using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using BizMall.Models.CompanyModels;
using Microsoft.AspNetCore.Mvc;

namespace BizMall.Models.CommonModels
{
    public class Image
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsMain { get; set; }
        public byte[] ImageContent { get; set; }
        [HiddenInput(DisplayValue = false)]
        public string ImageMimeType { get; set; }

        public List<RelGoodImage> Articles { get; set; }
        public List<RelCompanyImage> Companies { get; set; }

        public bool ToDelete { get; set; }

        public Image() {
            Articles = new List<RelGoodImage>();
            Companies = new List<RelCompanyImage>();
        } 
             

    }
}
