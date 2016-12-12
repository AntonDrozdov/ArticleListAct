﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BizMall.Models.CompanyModels;
using BizMall.Models.CommonModels;
using Microsoft.AspNetCore.Mvc;

namespace BizMall.ViewModels.AdminCompanyGoods
{
    public class CreateEditArticleViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        public string Title { get; set; }

        [Required(ErrorMessage = "Введите описание (от 6 до 3000 символов)")]
        [StringLength(3000, ErrorMessage = "Введите описание (от 6 до 3000 символов)", MinimumLength = 6)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Введите описание (от 6 до 3000 символов)")]
        [StringLength(3000, ErrorMessage = "Введите описание (от 6 до 3000 символов)", MinimumLength = 6)]
        [DataType(DataType.MultilineText)]
        public string HashTags { get; set; }

        [Required(ErrorMessage = "Введите описание (от 6 символов)")]
        [StringLength(3000, ErrorMessage = "Введите описание (от 10 символов)", MinimumLength = 10)]
        [DataType(DataType.Url)]
        public string Link { get; set; }

        //все про категории
        [Required(ErrorMessage = "Выбирете категорию")]
        [StringLength(100, ErrorMessage = "Выберете категорию", MinimumLength = 3)]
        public string Category { get; set; }
        public int? CategoryId { get; set; }

        //все про изображения
        public List<ImageViewModel> ImageViewModels { get;set;}
        public string MainImageInBase64 { get; set; }
        public string goodImagesIds { get; set; } //в формате id_id_id_id_...
        public string addedImagesIds { get; set; } //в формате id_id_id_id_...
        public string deletedImagesIds { get; set; } //в формате id_id_id_id_...

        public CreateEditArticleViewModel() {
            ImageViewModels = new List<ImageViewModel>();
        }
    }
}
