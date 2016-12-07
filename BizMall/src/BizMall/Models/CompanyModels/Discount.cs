using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace BizMall.Models.CompanyModels
{
    public class Discount
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.DateTime)]
        public DateTime BeginDate { get; set; }

        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.DateTime)]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(0, 100)]
        public int DiscountRate { get; set; }

        public ICollection<Article> Goods { get; set; }
        public Discount()
        {
            BeginDate = DateTime.Now;
            EndDate = DateTime.Now;
            DiscountRate = 0;
            Goods = new List<Article>();
        }

    }
}
