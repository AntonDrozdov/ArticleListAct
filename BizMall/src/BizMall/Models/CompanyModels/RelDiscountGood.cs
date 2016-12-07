using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BizMall.Models.CompanyModels
{
    public class RelDiscountGood
    {
        public int DiscountId { get; set; }
        public Discount Discount { get; set; }

        public int GoodId { get; set; }
        public Article Good { get; set; }
    }
}
