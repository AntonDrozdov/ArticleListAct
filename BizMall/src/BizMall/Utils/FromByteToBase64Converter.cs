using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BizMall.Models.CommonModels;

namespace BizMall.Utils
{
    public static class FromByteToBase64Converter
    {
        public static string GetImageBase64Src(Image image) {
            string imageBase64 = Convert.ToBase64String(image.ImageContent);
            return  string.Format("data:" + image.ImageMimeType + "; base64,{0}", imageBase64);
        }
    }
}
