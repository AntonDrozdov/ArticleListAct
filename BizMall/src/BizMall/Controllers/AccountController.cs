using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using BizMall.Models;
using BizMall.ViewModels.AccountViewModels;
using BizMall.Services;
using BizMall.Data.Repositories.Abstract;
using Microsoft.AspNetCore.Http;
using BizMall.Models.CommonModels;
using System.IO;
using BizMall.ViewModels.AdminCompanyArticles;
using BizMall.Utils;
using BizMall.Models.CompanyModels;
using Microsoft.Extensions.Options;

namespace BizMall.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly IRepositoryCompany _repositoryCompany;
        private readonly IRepositoryImage _repositoryImage;
        private readonly AppSettings _settings;


        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory,
            IRepositoryCompany repositoryCompany,
            IRepositoryImage repositoryImage,
             IOptions<AppSettings> settings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _repositoryCompany = repositoryCompany;
            _repositoryImage = repositoryImage;
            _settings = settings.Value;
        }

        #region POST/GET Login 
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {

            ViewData["Title"] = _settings.ApplicationTitle + "Вход";
            ViewData["HeaderTitle"] = _settings.HeaderTitle;
            ViewData["FooterTitle"] = _settings.FooterTitle;

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["Title"] = _settings.ApplicationTitle + "Вход";
            ViewData["HeaderTitle"] = _settings.HeaderTitle;
            ViewData["FooterTitle"] = _settings.FooterTitle;
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation(1, "User logged in.");
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, "User account locked out.");
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        #endregion

        #region POST/GET RegisterCompany 
        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterCompany(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            var cecvm = new CreateEditCompanyViewModel();

            ViewData["Title"] = _settings.ApplicationTitle + "Регистрация";
            ViewData["HeaderTitle"] = _settings.HeaderTitle;
            ViewData["FooterTitle"] = _settings.FooterTitle;

            return View(cecvm);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterCompany(CreateEditCompanyViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                    //    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(3, "User created a new account with password.");

                    #region ФОРМИРУЕМ СПИСОК ИЗОБРАЖЕНИЙ
                    //ФОРМИРУЕМ СПИСОК ИЗОБРАЖЕНИЙ
                    List<RelCompanyImage> relImages = new List<RelCompanyImage>();
                    //если строка id изображений непуста тогда формируем список
                    if (model.companyImagesIds!= null)
                    {
                        string[] strImgids = model.companyImagesIds.Trim().Substring(0, model.companyImagesIds.Length - 1).Split('_');
                        foreach (var strImageId in strImgids)
                        {
                            if (strImageId.Length == 0) continue;//это случай когдау товара нет изображений, но в массив все равно попадает распарсеная пустая строка
                            relImages.Add(new RelCompanyImage
                            {
                                CompanyId = 0,
                                ImageId = Convert.ToInt32(strImageId)
                            });
                        }
                    }
                    #endregion

                    #region СОЗДАНИЕ КОМПАНИИ
                    //При регистрации пользователя для него по умолчанию создается компания с параметрами которые он задал
                    _repositoryCompany.SaveCompanyAccount(new Company
                    {
                        ApplicationUserId = user.Id,
                        AccountType = AccountType.Company,
                        Title = model.Name,
                        Description = model.ActivityDescription,
                        ContactEmail = model.Email,
                        Telephone = model.Telephone,
                        Images = relImages
                    });
                    #endregion
                    return RedirectToAction(nameof(HomeController.IndexCat), "Home");
                }
                AddErrors(result);
            }

            #region ОТОБРАЖЕНИЕ УЖЕ ДОБАВЛЕННЫХ ФОТОК ЕСЛИ ОШИБКА ПРИ СОХРАНЕНИИ
            // If we got this far, something failed, redisplay form
            // заполняем список изображений уже добавленных пользователем при регистрации
            if (model.companyImagesIds != null)
            {
                List<int> logoids = GetIntIds.ConvertIdsToInt(model.companyImagesIds);
                model.LogoImageInBase64 = FromByteToBase64Converter.GetImageBase64Src(_repositoryImage.GetImage(logoids[0]));
                foreach (var id in logoids)
                {
                    Image im = _repositoryImage.GetImage(id);
                    //для каждого изображения составляем соответствующую модель отображения
                    model.ImageViewModels.Add(
                        new ImageViewModel
                        {
                            GoodId = 0,
                            Id = im.Id,
                            goodImageIds = "0_" + im.Id,
                            ImageMimeType = im.ImageMimeType,
                            ImageInBase64 = FromByteToBase64Converter.GetImageBase64Src(im)
                        }
                    );                    
                }
            }
            #endregion

            return View(model);
        }
        #endregion

        #region POST/GET RegisterPrivatePerson 
        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterPrivatePerson(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPrivatePerson(CreateEditCompanyViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                    //    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(3, "User created a new account with password.");

                    #region ФОРМИРУЕМ СПИСОК ИЗОБРАЖЕНИЙ
                    //ФОРМИРУЕМ СПИСОК ИЗОБРАЖЕНИЙ - для PrivatePerson он пустой
                    List<RelCompanyImage> relImages = new List<RelCompanyImage>();
                    #endregion

                    #region СОЗДАНИЕ КОМПАНИИ
                    //При регистрации пользователя для него по умолчанию создается компания с параметрами которые он задал
                    _repositoryCompany.SaveCompanyAccount(new Company
                    {
                        ApplicationUserId = user.Id,
                        AccountType = AccountType.PrivatePerson,
                        Title = model.Name,
                        Description = model.ActivityDescription,
                        ContactEmail = model.Email,
                        Telephone = model.Telephone,
                        Images = relImages
                    });
                    #endregion
                    return RedirectToAction(nameof(HomeController.IndexCat), "Home");
                }
                AddErrors(result);
            }
            return View(model);
        }
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(HomeController.IndexCat), "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        #region POST/GET ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                //var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                //var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                //await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                //   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
                //return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        #endregion

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        #region POST/GET ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            AddErrors(result);
            return View();
        }
        #endregion

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            // Generate the token and send it
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
            }

            var message = "Your security code is: " + code;
            if (model.SelectedProvider == "Email")
            {
                await _emailSender.SendEmailAsync(await _userManager.GetEmailAsync(user), "Security Code", message);
            }
            else if (model.SelectedProvider == "Phone")
            {
                await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
            }

            return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        #region POST/GET VerifyCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            // Require that the user has already logged in via username/password or external login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(7, "User account locked out.");
                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid code.");
                return View(model);
            }
        }
        #endregion

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                if (error.Code == "DuplicateUserName") error.Description = "Пользователь с таким Email уже зарегистрирован.";
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.IndexCat), "Home");
            }
        }

        #endregion

        #region FOR AJAX
        /// <summary>
        /// ajax:добавление на лету изображения к товару
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="newimages"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public JsonResult AddCompanyImage(int Id, ICollection<IFormFile> newimages)
        {
            //просто пишем изображение в бд
            Image image = new Image
            {
                Id = 0,
                IsMain = true,
                Description = "",
                ImageMimeType = newimages.ToList()[0].ContentType,
            };

            using (var reader = new StreamReader(newimages.ToList()[0].OpenReadStream()))
            {
                Stream stream = reader.BaseStream;
                Byte[] inArray = new Byte[(int)stream.Length];
                stream.Read(inArray, 0, (int)stream.Length);

                image.ImageContent = inArray;
                if (Id != 0)
                {
                    image.Companies.Add(new Models.CompanyModels.RelCompanyImage
                    {
                        ImageId = image.Id,
                        CompanyId = Id
                    });
                }
            }

            //картинки в бд
            return Json(_repositoryImage.SaveImage(image));
        }

        /// <summary>
        /// ajax:используестя после успешного добавлениия изображения в бД для формирования превью
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetImageForThumb(int Id)
        {
            Image image = _repositoryImage.GetImage(Id);

            ImageViewModel imageViewModel = new ImageViewModel
            {
                GoodId = 0,
                Id = image.Id,
                goodImageIds = 0 + "_" + image.Id,
                ImageMimeType = image.ImageMimeType,
                ImageInBase64 = FromByteToBase64Converter.GetImageBase64Src(image)
            };

            return Json(imageViewModel);
        }

        /// <summary>
        /// ajax:удаление на лету изображения к товару
        /// </summary>
        /// <param name="goodImageIds"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public string DeleteCompanyImage(string companyImagesIds)
        {
            if (companyImagesIds != null)
            {
                string[] parameteres = companyImagesIds.Split('_');

                int goodId = Convert.ToInt32(parameteres[0]);
                int imageId = Convert.ToInt32(parameteres[1]);
                _repositoryImage.DeleteImage(imageId);

                return imageId.ToString();//для того чтобы front переделал строку id зиображений товара в актуальную
            }
            return null;
        }
        /// <summary>
        /// ajax:удаление на лету изображения к товару
        /// </summary>
        /// <param name="goodImageIds"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public bool DeleteCompanyImages(string companyImagesIds)
        {
            if (companyImagesIds != null)
            {
                int[] ids = GetIntIds.ConvertIdsToInt(companyImagesIds).ToArray();
                _repositoryImage.DeleteImages(ids);
            }
            return true;
        }
        #endregion
    }
}
