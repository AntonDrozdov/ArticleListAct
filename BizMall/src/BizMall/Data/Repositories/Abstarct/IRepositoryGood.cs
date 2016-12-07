using System;
using System.Collections.Generic;
using System.Linq;

using BizMall.Models.CompanyModels;
using Microsoft.AspNetCore.Http;

namespace BizMall.Data.Repositories.Abstract
{
    public interface IRepositoryGood
    {
        Article GetGood(int goodId);
        void DeleteGood(int goodId);
        IQueryable<Article> ShopGoodsFullInformation(int ShopId);
        IQueryable<Article> ShopGoods(int ShopId);
        Article SaveGood(Article good, Company company);
        void ArchieveGoods(List<int> ids);
        void ActivateGoods(List<int> ids);
    }
}
