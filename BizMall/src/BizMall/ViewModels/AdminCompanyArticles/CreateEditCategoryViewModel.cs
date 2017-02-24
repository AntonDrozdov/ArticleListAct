using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BizMall.Models.CompanyModels;
using BizMall.Models.CommonModels;
using Microsoft.AspNetCore.Mvc;

namespace BizMall.ViewModels.AdminCompanyArticles
{
    public class CreateEditCategoryViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название (от 3 до 3000 символов)")]
        [StringLength(3000, ErrorMessage = "Введите название (от 3 до 3000 символов)", MinimumLength = 3)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Введите название(латиница) (от 3 до 3000 символов)")]
        [StringLength(3000, ErrorMessage = "Введите название(латиница) (от 3 до 3000 символов)", MinimumLength = 3)]
        public string EnTitle { get; set; }

        [Required(ErrorMessage = "Введите название (от 3 до 3000 символов)")]
        [StringLength(3000, ErrorMessage = "Введите название (от 3 до 3000 символов)", MinimumLength = 3)]
        public string ParentCategory { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}
