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
    public class CreateEditKWViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите ключевик (от 3 до 100 символов)")]
        [StringLength(100, ErrorMessage = "Введите ключевик (от 3 до 100 символов)", MinimumLength = 3)]
        public string kw { get; set; }

        public int? CategoryId { get; set; }
        public string CategoryTitle { get; set; }

        public CategoryType CategoryType { get; set; }

        [Required(ErrorMessage = "Введите данные для Meta тега KeyWords")]
        [StringLength(100, ErrorMessage = "Введите данные для Meta тега KeyWords", MinimumLength = 2)]
        public string metaKeyWords { get; set; }

        [Required(ErrorMessage = "Введите данные для Meta тега Description")]
        [StringLength(100, ErrorMessage = "Введите данные для Meta тега Description", MinimumLength = 2)]
        public string metaDescription { get; set; }
    }
}
