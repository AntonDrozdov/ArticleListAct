using BizMall.Models.CommonModels;

namespace BizMall.Models.CompanyModels
{
    public class RelCompanyImage
    {
        public int ImageId { get; set; }
        public Image Image { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
