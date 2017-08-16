using System;
using System.Collections.Generic;
using System.Linq;

using BizMall.Data.Repositories.Abstract;
using BizMall.ViewModels.AdminCompanyArticles;
using BizMall.Models.CompanyModels;
using BizMall.Models.CommonModels;
using BizMall.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using ArticleList.Models.CommonModels;
using Microsoft.Extensions.Options;

namespace BizMall.Controllers
{
    /// <summary>
    /// ctor
    /// </summary>
    [Authorize]
    public class AdminArticlesController : Controller
    {
        private readonly IRepositoryUser _repositoryUser;
        private readonly IRepositoryCompany _repositoryCompany;
        private readonly IRepositoryArticle _repositoryArticle;
        private readonly IRepositoryImage _repositoryImage;
        private readonly IRepositoryKW _repositoryKW;
        private readonly IRepositoryCategory _repositoryCategory;
        private readonly AppSettings _settings;

        public AdminArticlesController(IRepositoryUser repositoryUser,
                                            IRepositoryCompany repositoryCompany,
                                            IRepositoryArticle repositoryArticle,
                                            IRepositoryImage repositoryImage,
                                            IRepositoryKW repositoryKW,
                                            IRepositoryCategory repositoryCategory,
                                            IOptions<AppSettings> settings)
        {
            _repositoryUser = repositoryUser;
            _repositoryCompany = repositoryCompany;
            _repositoryArticle = repositoryArticle;
            _repositoryImage = repositoryImage;
            _repositoryKW = repositoryKW;
            _repositoryCategory = repositoryCategory;
            _settings = settings.Value;
        }

        #region статьи: отображени, редактирование, удаление

        /// <summary>
        /// вывод товаров в личный кабинет компании
        /// </summary>
        public IActionResult Articles(string Category, int Page = 1)
        {
            var currentUser = _repositoryUser.GetCurrentUser(User.Identity.Name);

            if (currentUser != null)
            {
                var Company = _repositoryCompany.GetUserCompany(currentUser);

                PagingInfo pagingInfo;
                var Items = _repositoryArticle.CompanyArticlesFullInformation(Company.Id, Category, Page, out pagingInfo).ToList();

                List<ArticleViewModel> ArticlesVM = new List<ArticleViewModel>();
                foreach (var item in Items)
                {
                    var avm = ConstructAVM(item);
                    ArticlesVM.Add(avm);
                }

                ViewData["Title"] = _settings.ApplicationTitle + "Администрирование: Статьи";
                ViewData["HeaderTitle"] = _settings.HeaderTitle;
                ViewData["FooterTitle"] = _settings.FooterTitle;
                ViewBag.ArticlesVM = ArticlesVM;
                ViewBag.PagingInfo = pagingInfo;
                ViewBag.ActiveSubMenu = "Статьи";
            }
            else
            {
                Redirect("/");
            }

            return View();
        }

        /// <summary>
        /// поиск по списку редакитруемых статей
        /// </summary>
        public IActionResult Search(string searchstring, int Page = 1)
        {
            if (searchstring == null)
                return RedirectToAction("Articles");

            PagingInfo pagingInfo;
            var Items = _repositoryArticle.SearchStringAdminArticlesFullInformation(searchstring, Page, out pagingInfo).ToList();

            List<ArticleViewModel> ArticlesVM = new List<ArticleViewModel>();
            foreach (var item in Items)
            {
                var avm = ConstructAVM(item);
                ArticlesVM.Add(avm);
            }

            ViewData["Title"] = _settings.ApplicationTitle + "Поиск: " + searchstring;
            ViewData["HeaderTitle"] = _settings.HeaderTitle;
            ViewData["FooterTitle"] = _settings.FooterTitle;
            ViewBag.ArticlesVM = ArticlesVM;
            ViewBag.PagingInfo = pagingInfo;
            ViewBag.ActiveSubMenu = "Статьи";

            return View();
        }

        /// <summary>
        /// редактирование товара
        /// </summary>
        [HttpGet]
        public IActionResult EditArticle(int id)
        {
            CreateEditArticleViewModel ceavm = null;
            if (id != 0)
            {
                // формирование данных статьи для отображения в интерфейсе редактирования
                Article item = _repositoryArticle.GetArticle(id);

                ceavm = ConstructCEAVM(item);

                if (item.Images.Count != 0)
                {
                    ceavm.MainImageInBase64 = FromByteToBase64Converter.GetImageBase64Src(item.Images.ToList()[0].Image);
                    foreach (var rgi in item.Images)
                    {
                        //для каждого изображения составляем соответствующую модель отображения
                        ceavm.ImageViewModels.Add(ConstructIVM(rgi));

                        //для каждого изображения оставляем его id в input всех id изображений товара
                        ceavm.goodImagesIds += rgi.ImageId + "_";
                    }
                }

            }
            else
                ceavm = new CreateEditArticleViewModel();

            //формирование списка ключевиков в поле "ДЛЯ САЙТА"
            ViewBag.SiteKws = _repositoryKW.Kws(null).ToList();

            ViewData["Title"] = _settings.ApplicationTitle +"Администрирование: Добавление/Редактирование статьи";
            ViewData["HeaderTitle"] = _settings.HeaderTitle;
            ViewData["FooterTitle"] = _settings.FooterTitle;
            ViewBag.ActiveSubMenu = "Статьи";

            return View(ceavm);
        }

        [HttpPost]
        public IActionResult EditArticle(CreateEditArticleViewModel model)
        {
            if (ModelState.IsValid)
            {
                //текущ
                var currentUser = _repositoryUser.GetCurrentUser(User.Identity.Name);

                //получаем компанию родителя определяемую текущий пользователем
                Company company = new Company();
                if (currentUser != null)
                    company = _repositoryCompany.GetUserCompany(currentUser);
                else
                    return RedirectToAction("Articles");

                //формируем список изображений
                List<RelGoodImage> relImages = ConstructImagegeListFromCEAVM(model);

                //сохранение статьи
                SaveArticleChanges(company, model, relImages);

                //подчищаем таблицу изображений
                CleanDBFromDeletedImages(model);

            }
            return RedirectToAction("Articles");
        }

        /// <summary>
        /// удаление товара
        /// </summary>
        /// <param name="goodId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult DeleteArticle(int itemId)
        {
            if (itemId != 0)
            {
                _repositoryArticle.DeleteArticle(itemId);
            }
            return RedirectToAction("Articles");
        }

        #endregion

        /// <summary>
        /// ключевики категории
        /// </summary>
        [HttpPost]
        public JsonResult CategoryKws(string CategoryId)
        {
            if (CategoryId != null)
            {
                var categoryKws = _repositoryKW.Kws(CategoryId).ToList();
                return Json(categoryKws);
            }
            return Json(null);
        }

        #region активация/деактивация товаров

        /// <summary>
        /// ajax:активация/деактивация товаров
        /// </summary>
        //public bool ArchieveGoods(string checkedGoods)
        //{
        //    _repositoryGood.ArchieveGoods(GetIntIds.ConvertIdsToInt(checkedGoods));
        //
        //    //return RedirectToAction("Goods", new { goodsStatus = GoodStatus.Active });
        //    return true;
        //}
        //
        //public bool ActivateGoods(string checkedGoods)
        //{
        //    _repositoryGood.ActivateGoods(GetIntIds.ConvertIdsToInt(checkedGoods));
        //
        //    return true;
        //    //return RedirectToAction("Goods", new { goodsStatus = GoodStatus.InActive});
        //}

        #endregion

        #region Работа с изображниями - добавление/удалении, редактировани, "Назад"

        /// <summary>
        /// выбрать главное изображение товара
        /// </summary>
        public FileContentResult GetGoodMainImage(int GoodId)
        {
            Image image = _repositoryImage.GetGoodImage(GoodId);
            var fcr = File(image.ImageContent, image.ImageMimeType);
            return fcr;
        }



        /// <summary>
        /// ajax:добавление на лету изображения к товару
        /// </summary>
        [HttpPost]
        public JsonResult AddArticleImage(int Id, ICollection<IFormFile> newimages)
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
                    image.Articles.Add(new RelGoodImage
                    {
                        ImageId = image.Id,
                        GoodId = Id
                    });
                }
            }

            //картинки в бд
            return Json(_repositoryImage.SaveImage(image));
        }

        /// <summary>
        ///  ajax:используестя после успешного добавлениия изображения в бД для формирования превью
        /// </summary>
        [HttpGet]
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
        [HttpPost]
        public string DeleteArticleImage(string goodImageIds)
        {
            if (goodImageIds != null)
            {
                string[] parameteres = goodImageIds.Split('_');

                int goodId = Convert.ToInt32(parameteres[0]);
                int imageId = Convert.ToInt32(parameteres[1]);
                _repositoryImage.ChangeImageToDeleteStatus(imageId);

                return imageId.ToString();//для того чтобы front переделал строку id зиображений товара в актуальную
            }
            return null;
        }

        /// <summary>
        /// ajax:восстановление/удаление фоток при "Назад"
        /// </summary>
        [HttpPost]
        public string RestoreImages(string goodImageIds, string addedImagesIds, string deletedImagesIds)
        {
            if (deletedImagesIds != null)
            {
                int[] ids = GetIntIds.ConvertIdsToInt(deletedImagesIds).ToArray();
                _repositoryImage.ChangeImagesToNonDeleteStatus(ids);               
            }
            if (addedImagesIds!=null)
            {
                int[] ids = GetIntIds.ConvertIdsToInt(addedImagesIds).ToArray();
                _repositoryImage.DeleteImages(ids);
            }
            return "success";//для того чтобы front переделал строку id зиображений товара в актуальную
        }

        /// <summary>
        /// ajax:удаление добавленных на лету изображений товара в случае если пользователь нажал "Назад"
        /// </summary>
        [HttpPost]
        public bool DeleteArticleImages(string goodImageIds)
        {
            if (goodImageIds != null)
            {
                int[] ids = GetIntIds.ConvertIdsToInt(goodImageIds).ToArray();
                _repositoryImage.DeleteImages(ids);
            }
            return true;
        }

        #endregion

        #region PRIVATE METHODS

        private ArticleViewModel ConstructAVM(Article item)
        {
            return new ArticleViewModel
            {
                Id = item.Id,
                Title = item.Title,
                EnTitle = item.EnTitle,
                Description = item.Description,
                UpdateTime = item.UpdateTime,
                Category = item.Category,
                CategoryId = item.CategoryId
            };
        }

        private CreateEditArticleViewModel ConstructCEAVM(Article item)
        {
            return new CreateEditArticleViewModel
            {
                Title = item.Title,
                EnTitle = item.EnTitle,
                Description = item.Description,
                Link = item.Link,
                HashTags = item.HashTags,
                Category = item.Category.Title,
                CategoryId = item.CategoryId,
                Id = item.Id,
                metaDescription = item.metaDescription,
                metaKeyWords = item.metaKeyWords
            };
        }

        private ImageViewModel ConstructIVM(RelGoodImage rgi)
        {
            return new ImageViewModel
            {
                GoodId = rgi.GoodId,
                Id = rgi.ImageId,
                goodImageIds = rgi.GoodId + "_" + rgi.ImageId,
                ImageMimeType = rgi.Image.ImageMimeType,
                ImageInBase64 = FromByteToBase64Converter.GetImageBase64Src(rgi.Image)
            };
        }

        private List<RelGoodImage> ConstructImagegeListFromCEAVM(CreateEditArticleViewModel model)
        {
            List<RelGoodImage> relImages = new List<RelGoodImage>();

            if (model.goodImagesIds != null)
            {
                string[] strImgids = model.goodImagesIds.Trim().Substring(0, model.goodImagesIds.Length - 1).Split('_');
                foreach (var strImageId in strImgids)
                {
                    if (strImageId.Length == 0) continue;//это случай когдау товара нет изображений, но в массив все равно попадает распарсеная пустая строка
                    relImages.Add(new RelGoodImage
                    {
                        GoodId = model.Id,
                        ImageId = Convert.ToInt32(strImageId)
                    });
                }
            }

            return relImages;
        }

        private void SaveArticleChanges(Company company, CreateEditArticleViewModel model, List<RelGoodImage> relImages)
        {
            _repositoryArticle.SaveArticle(new Article
                {
                    Id = model.Id,
                    Title = model.Title,
                    EnTitle = model.EnTitle,
                    Description = model.Description,
                    Link = model.Link,
                    HashTags = model.HashTags,
                    CategoryId = Convert.ToInt32(model.CategoryId),
                    CategoryType = _repositoryCategory.GetCategoryById(Convert.ToInt32(model.CategoryId)).CategoryType,
                    Images = relImages,
                    UpdateTime = DateTime.Now,
                    metaDescription = model.metaDescription,
                    metaKeyWords = model.metaKeyWords
                },
                company);
        }

        private void CleanDBFromDeletedImages(CreateEditArticleViewModel model)
        {
            if (model.deletedImagesIds != null)
            {
                int[] ids = GetIntIds.ConvertIdsToInt(model.deletedImagesIds).ToArray();
                _repositoryImage.DeleteImages(ids);
            }
        }

        #endregion
    }
}
