using BizMall.ViewModels.AdminCompanyArticles;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BizMall.ViewModels.AccountViewModels
{
    public class CreateEditCompanyViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "ActivityDescription")]
        public string ActivityDescription { get; set; }

        [Required]
        [Display(Name = "Telephone")]
        public string Telephone { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        //все про изображения
        public List<ImageViewModel> ImageViewModels { get; set; }
        public string LogoImageInBase64 { get; set; }
        public string companyImagesIds { get; set; } //в формате id_id_id_id_...

        public CreateEditCompanyViewModel()
        {
            ImageViewModels = new List<ImageViewModel>();
        }

    }
}
