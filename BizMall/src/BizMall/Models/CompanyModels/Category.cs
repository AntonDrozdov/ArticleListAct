using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace BizMall.Models.CompanyModels
{
    public class Category
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public int? CategoryId { get; set; }
        public Category ParentCategory { get; set; }

        public ICollection<Good> Goods { get; set; }

        public Category()
        {
            Goods = new List<Good>();
        }

    }
}
