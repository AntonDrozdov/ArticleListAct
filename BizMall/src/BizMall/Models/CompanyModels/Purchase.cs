using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace BizMall.Models.CompanyModels
{
    public class Purchase
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.DateTime)]
        public DateTime Date { get; set; }

        [Required]
        [Range(0, 1000)]
        public int? Amount { get; set; }

        [Required]
        public double PurchasePricePerItem { get; set; }

        public int? GoodId { get; set; }
        public Article Good { get; set; }

        public Purchase()
        {
            Amount = 0;
        }
    }
}
