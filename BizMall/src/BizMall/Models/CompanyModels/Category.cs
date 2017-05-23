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

        [Required(ErrorMessage = "Введите название (от 3 до 100 символов)")]
        [StringLength(100, ErrorMessage = "Введите название (от 3 до 100 символов)", MinimumLength = 3)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Введите название(латиница) (от 3 до 100 символов)")]
        [StringLength(100, ErrorMessage = "Введите название(латиница) (от 3 до 100 символов)", MinimumLength = 3)]
        public string EnTitle { get; set; }

        public int? CategoryId { get; set; }
        public Category ParentCategory { get; set; }

        public ICollection<Article> Goods { get; set; }

        public CategoryType CategoryType { get; set; }

        public Category()
        {
            Goods = new List<Article>();
        }

        //для meta - тегов

        public string metaKeyWords { get; set; }

        public string metaDescription { get; set; }


    }
}
