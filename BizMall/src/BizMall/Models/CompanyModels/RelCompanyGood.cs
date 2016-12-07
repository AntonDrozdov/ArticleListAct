using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BizMall.Models.CompanyModels
{
    public class RelCompanyGood
    {
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public int GoodId { get; set; }
        public Article Good {get;set;}
    }
}
