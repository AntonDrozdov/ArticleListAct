using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace BizMall.Models.CompanyModels
{
    public class KW
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название (от 3 до 100 символов)")]
        [StringLength(100, ErrorMessage = "Введите название (от 3 до 100 символов)", MinimumLength = 3)]
        public string kw { get; set; }

        public int? CategoryId { get; set; }
        public Category Category { get; set; }

        public CategoryType CategoryType { get; set; }
    }
}
