using System;
using System.Collections.Generic;
using System.Linq;

using BizMall.Data.Repositories.Abstract;
using BizMall.ViewModels.AdminCompanyGoods;
using BizMall.Models.CompanyModels;
using BizMall.Models.CommonModels;
using BizMall.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace BizMall.Controllers
{
    /// <summary>
    /// ctor
    /// </summary>
    public class AdminCompanyGoodsController : Controller
    {
        private readonly IRepositoryUser _repositoryUser;
        private readonly IRepositoryCompany _repositoryCompany;
        private readonly IRepositoryGood _repositoryGood;
        private readonly IRepositoryImage _repositoryImage;

        public AdminCompanyGoodsController(IRepositoryUser repositoryUser,
                                            IRepositoryCompany repositoryCompany,
                                            IRepositoryGood repositoryGood,
                                            IRepositoryImage repositoryImage)
        {
            _repositoryUser = repositoryUser;
            _repositoryCompany = repositoryCompany;
            _repositoryGood = repositoryGood;
            _repositoryImage = repositoryImage;
        }

        /// <summary>
        /// выбрать главное изображение товара
        /// </summary>
        /// <param name="GoodId"></param>
        /// <returns></returns>
        public FileContentResult GetGoodMainImage(int GoodId)
        {
            Image image = _repositoryImage.GetGoodImage(GoodId);
            var fcr = File(image.ImageContent, image.ImageMimeType);
            return fcr;
        }

        /// <summary>
        /// вывод товаров в личный кабинет компании
        /// </summary>
        /// <param name="goodsStatus"></param>
        /// <returns></returns>
        public IActionResult Goods(GoodStatus goodsStatus = GoodStatus.Active)
        {
            var currentUser = _repositoryUser.GetCurrentUser(User.Identity.Name);

            if (currentUser != null)
            {
                var Company = _repositoryCompany.GetUserCompany(currentUser);
                var Goods = _repositoryGood.ShopGoodsFullInformation(Company.Id, goodsStatus).ToList();

                List<GoodViewModel> GoodsVM = new List<GoodViewModel>();
                foreach (var good in Goods)
                {
                    var iDaysToSetInActiveStatus = 31 - (DateTime.Now - good.UpdateTime).Days;
                    GoodViewModel gvm = new GoodViewModel
                    {
                        Amount = good.Amount,
                        Category = good.Category,
                        CategoryId = good.CategoryId,
                        Companies = good.Companies,
                        Description = good.Description,
                        Id = good.Id,
                        Title = good.Title,
                        DaysToSetInActiveStatus = iDaysToSetInActiveStatus
                    };
                    if (good.Images.Count != 0)
                        gvm.MainImageInBase64 = FromByteToBase64Converter.GetImageBase64Src(good.Images[0].Image);

                    GoodsVM.Add(gvm);
                }
                ViewBag.GoodsVM = GoodsVM;
            }
            else
            {
                Redirect("/");
            }

            ViewBag.ActiveSubMenu = "Товары/Услуги";
            if (goodsStatus == GoodStatus.Active)
                ViewBag.ActiveGoodsStatusMenu = 1;
            if (goodsStatus == GoodStatus.InActive)
                ViewBag.ActiveGoodsStatusMenu = 0;
            return View();
        }

        /// <summary>
        /// редактирование товара
        /// </summary>
        /// <param name="id"> id товара</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult EditGood(int id)
        {
            CreateEditGoodViewModel cegvm = null;
            if (id != 0)
            {
                Good good = _repositoryGood.GetGood(id);

                cegvm = new CreateEditGoodViewModel
                {
                    Category = good.Category.Title,
                    CategoryId = good.CategoryId,
                    Description = good.Description,
                    Id = good.Id,
                    Title = good.Title
                };
                if (good.Images.Count != 0)
                {
                    cegvm.MainImageInBase64 = FromByteToBase64Converter.GetImageBase64Src(good.Images.ToList()[0].Image);
                    foreach (var rgi in good.Images)
                    {
                        //для каждого изображения составляем соответствующую модель отображения
                        cegvm.ImageViewModels.Add(
                            new ImageViewModel
                            {
                                GoodId = rgi.GoodId,
                                Id = rgi.ImageId,
                                goodImageIds = rgi.GoodId + "_" + rgi.ImageId,
                                ImageMimeType = rgi.Image.ImageMimeType,
                                ImageInBase64 = FromByteToBase64Converter.GetImageBase64Src(rgi.Image)
                            }
                        );
                        //для каждого изображения оставляем его id в input всех id изображений товара
                        cegvm.goodImagesIds += rgi.ImageId + "_";
                    }
                }
            }
            else
                cegvm = new CreateEditGoodViewModel();

            return View(cegvm);
        }
        [HttpPost]
        public IActionResult EditGood(CreateEditGoodViewModel model)
        {
            if (ModelState.IsValid)
            {
                //ТЕКУЩИЙ ПОЛЬЗОВАТЕЛЬ 
                var currentUser = _repositoryUser.GetCurrentUser(User.Identity.Name);

                //ПОЛУЧАЕМ КОМПАНИЮ РОДИТЕЛЯ ОПРЕДЕЛЯЕМУЮ ТЕКУЩИМ ПОЛЬЗОВАТЕЛЕМ 
                Company company = new Company();
                if (currentUser != null)
                    company = _repositoryCompany.GetUserCompany(currentUser);
                else
                    return RedirectToAction("Goods");

                //ФОРМИРУЕМ СПИСОК ИЗОБРАЖЕНИЙ
                List<RelGoodImage> relImages = new List<RelGoodImage>();
                //если строка id изображений непуста тогда формируем список
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

                _repositoryGood.SaveGood(new Good
                {
                    Id = model.Id,
                    Title = model.Title,
                    Description = model.Description,
                    CategoryId = Convert.ToInt32(model.CategoryId),
                    Images = relImages,
                    UpdateTime = DateTime.Now
                },
                company);
                if (model.deletedImagesIds != null)
                {
                    int[] ids = GetIntIds.ConvertIdsToInt(model.deletedImagesIds).ToArray();
                    _repositoryImage.DeleteImages(ids);
                }
            }
            return RedirectToAction("Goods");
        }

        /// <summary>
        /// удаление товара
        /// </summary>
        /// <param name="goodId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult DeleteGood(int goodId, GoodStatus goodsStatus)
        {
            if (goodId != 0)
            {
                _repositoryGood.DeleteGood(goodId);
            }
            return RedirectToAction("Goods", new { goodsStatus = goodsStatus });
        }

        //ДЛЯ ajax

        /// <summary>
        /// ajax:деактивация товаров
        /// </summary>
        /// <param name="checkedGoods"></param>
        /// <returns></returns>
        public bool ArchieveGoods(string checkedGoods)
        {
            _repositoryGood.ArchieveGoods(GetIntIds.ConvertIdsToInt(checkedGoods));

            //return RedirectToAction("Goods", new { goodsStatus = GoodStatus.Active });
            return true;
        }

        /// <summary>
        /// ajax:активация товаров
        /// </summary>
        /// <param name="checkedGoods"></param>
        /// <returns></returns>
        public bool ActivateGoods(string checkedGoods)
        {
            _repositoryGood.ActivateGoods(GetIntIds.ConvertIdsToInt(checkedGoods));

            return true;
            //return RedirectToAction("Goods", new { goodsStatus = GoodStatus.InActive});
        }

        /// <summary>
        /// ajax:добавление на лету изображения к товару
        /// </summary>
        /// <param name="Id">id товара</param>
        /// <param name="newimages"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddGoodImage(int Id, ICollection<IFormFile> newimages)
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
                    image.Goods.Add(new RelGoodImage
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
        /// <param name="Id"></param>
        /// <returns></returns>
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
        /// <param name="goodImageIds"></param>
        /// <returns></returns>
        [HttpPost]
        public string DeleteGoodImage(string goodImageIds)
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
        /// <param name="goodImageIds"></param>
        /// <returns></returns>
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
        /// <param name="goodImageIds"></param>
        /// <returns></returns>
        [HttpPost]
        public bool DeleteGoodImages(string goodImageIds)
        {
            if (goodImageIds != null)
            {
                int[] ids = GetIntIds.ConvertIdsToInt(goodImageIds).ToArray();
                _repositoryImage.DeleteImages(ids);
            }
            return true;
        }
    }
}
