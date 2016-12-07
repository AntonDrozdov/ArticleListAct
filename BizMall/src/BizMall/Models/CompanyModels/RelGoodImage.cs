using BizMall.Models.CommonModels;

namespace BizMall.Models.CompanyModels
{
    public class RelGoodImage
    {
        public int ImageId { get; set; }
        public Image Image { get; set; }

        public int GoodId { get; set; }
        public Article Good { get; set; }
    }
}
