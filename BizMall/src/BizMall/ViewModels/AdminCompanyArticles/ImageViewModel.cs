using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BizMall.ViewModels.AdminCompanyArticles
{
    public class ImageViewModel
    {
        public int Id { get; set; }

        public int? GoodId { get; set; }

        public string goodImageIds { get; set; }

        public string ImageInBase64 { get; set; }
        public string ImageMimeType { get; set; }
    }
}
