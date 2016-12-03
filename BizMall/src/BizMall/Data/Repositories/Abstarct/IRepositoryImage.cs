using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BizMall.Models.CommonModels;

namespace BizMall.Data.Repositories.Abstract
{
    public interface IRepositoryImage
    {
        Image SaveImage(Image item);
        Image GetImage(int ImageId);
        void DeleteImage(int ImageId);
        void ChangeImageToDeleteStatus(int ImageId);
        void ChangeImagesToNonDeleteStatus(int[] ids);
        void DeleteImages(int[] ids);
        Image GetGoodImage(int GoodId);
    }
}
