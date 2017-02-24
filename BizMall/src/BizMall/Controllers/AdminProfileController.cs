using System;
using System.Collections.Generic;
using System.Linq;

using BizMall.Data.Repositories.Abstract;
using BizMall.Models.CompanyModels;
using Microsoft.AspNetCore.Mvc;
using BizMall.ViewModels.AccountViewModels;
using BizMall.Utils;
using BizMall.ViewModels.AdminCompanyArticles;
using Microsoft.AspNetCore.Identity;
using BizMall.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BizMall.Controllers
{
    [Authorize]
    public class AdminProfileController : Controller
    {
        private readonly IRepositoryUser _repositoryUser;
        private readonly IRepositoryCompany _repositoryCompany;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppSettings _settings;

        public AdminProfileController(   IRepositoryUser repositoryUser, 
                                                IRepositoryCompany repositoryCompany, 
                                                UserManager<ApplicationUser> userManager, 
                                                SignInManager<ApplicationUser> signInManager,
                                                IOptions<AppSettings> settings) {
            _repositoryUser = repositoryUser;
            _repositoryCompany = repositoryCompany;
            _userManager = userManager;
            _signInManager = signInManager;
            _settings = settings.Value;
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            ViewBag.ActiveSubMenu = "Профиль";
            ViewData["Title"] = _settings.ApplicationTitle + "Администрирование: Статьи";
            ViewData["HeaderTitle"] = _settings.HeaderTitle;
            ViewData["FooterTitle"] = _settings.FooterTitle;

            var user = _repositoryUser.GetCurrentUser(User.Identity.Name);
            var Company = _repositoryCompany.GetUserCompany(user);

            switch (Company.AccountType)
            {
                case AccountType.Company:
                {
                    #region редирект на view для Company
                     var cecvm = new CreateEditCompanyViewModel
                    {
                        Id = Company.Id,
                        Name = Company.Title,
                        ActivityDescription = Company.Description,
                        Email = Company.ContactEmail,
                        Telephone = Company.Telephone
                    };
                    #region ЗАПИСЬ ИЗОБРАЖЕНИЙ ВО VIEWMODEL
                    if (Company.Images.Count != 0) {
                        cecvm.LogoImageInBase64 = FromByteToBase64Converter.GetImageBase64Src(Company.Images.ToList()[0].Image);
                        foreach (var rci in Company.Images)
                        {
                            //для каждого изображения составляем соответствующую модель отображения
                            cecvm.ImageViewModels.Add(
                                new ImageViewModel
                                {
                                    GoodId = rci.CompanyId,
                                    Id = rci.ImageId,
                                    goodImageIds = rci.CompanyId + "_" + rci.ImageId,
                                    ImageMimeType = rci.Image.ImageMimeType,
                                    ImageInBase64 = FromByteToBase64Converter.GetImageBase64Src(rci.Image)
                                }
                            );
                            //для каждого изображения оставляем его id в input всех id изображений товара
                            cecvm.companyImagesIds += rci.ImageId + "_";
                        }
                    }
                    #endregion
                    return View("CompanyProfileEditView", cecvm);
                    #endregion
                }
                case AccountType.PrivatePerson:
                {
                    #region редирект на view для PrivatePerson
                    var cecvm = new CreateEditCompanyViewModel
                    {
                        Name = Company.Title,
                        ActivityDescription = Company.Description,
                        Email = Company.ContactEmail,
                        Telephone = Company.Telephone
                    };

                    return View("PrivatePersonProfileEditView", cecvm);
                    #endregion
                }
                default: 
                    return View("PrivatePersonProfileEditView");
                    
            }
        }
        [HttpPost]
        public async System.Threading.Tasks.Task<RedirectToActionResult> EditProfile(CreateEditCompanyViewModel model)
        {
            //if (ModelState.IsValid)
            //{
            var user = _repositoryUser.GetCurrentUser(User.Identity.Name);
            var Company = _repositoryCompany.GetUserCompanyWithNoTrackingImages(user);

            #region ФОРМИРУЕМ СПИСОК ИЗОБРАЖЕНИЙ
            //ФОРМИРУЕМ СПИСОК ИЗОБРАЖЕНИЙ
            List<RelCompanyImage> relImages = new List<RelCompanyImage>();
            //если строка id изображений непуста тогда формируем список
            if (model.companyImagesIds != null)
            {
                string[] strImgids = model.companyImagesIds.Trim().Substring(0, model.companyImagesIds.Length - 1).Split('_');
                foreach (var strImageId in strImgids)
                {
                    if (strImageId.Length == 0) continue;//это случай когдау товара нет изображений, но в массив все равно попадает распарсеная пустая строка
                    relImages.Add(new RelCompanyImage
                    {
                        CompanyId = Company.Id,
                        ImageId = Convert.ToInt32(strImageId)
                    });
                }
            }
            #endregion

            //если меняем email
            if (user.Email != model.Email)
            {
                user.Email = model.Email;
                user.UserName = model.Email;
                await _userManager.UpdateAsync(user);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            #region ФОРМИРУЕМ ИЗМЕНЕНННУЮ КОМПАНИЮ
            Company.Title = model.Name;
            Company.Description = model.ActivityDescription;
            Company.ContactEmail = model.Email;
            Company.Telephone = model.Telephone;
            Company.Images = relImages;
            #endregion

            _repositoryCompany.SaveCompanyAccount(Company);
            //}
            return RedirectToAction("EditProfile");
        }
    }
}
