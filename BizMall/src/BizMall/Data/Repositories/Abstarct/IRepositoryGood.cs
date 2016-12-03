using System;
using System.Collections.Generic;
using System.Linq;

using BizMall.Models.CompanyModels;
using Microsoft.AspNetCore.Http;

namespace BizMall.Data.Repositories.Abstract
{
    public interface IRepositoryGood
    {
        Good GetGood(int goodId);
        void DeleteGood(int goodId);
        IQueryable<Good> ShopGoodsFullInformation(int ShopId, GoodStatus goodsStatus);
        IQueryable<Good> ShopGoods(int ShopId);
        Good SaveGood(Good good, Company company);
        void ArchieveGoods(List<int> ids);
        void ActivateGoods(List<int> ids);
    }
}
