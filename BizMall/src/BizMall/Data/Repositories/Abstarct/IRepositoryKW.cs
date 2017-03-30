using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BizMall.Models.CompanyModels;
using ArticleList.Models.CommonModels;

namespace BizMall.Data.Repositories.Abstract
{
    public interface IRepositoryKW
    {
        IQueryable<KW> Kws(string Category);
        IQueryable<KW> Kws(string Category, int page, out PagingInfo pagingInfo);
        IQueryable<KW> SearchStringKws(string searchstring, int page, out PagingInfo pagingInfo);
        KW SaveKW(KW model);
        void DeleteKW(int itemId);
        KW GetKwById(int id);
        IQueryable<KW> KwsForTagCloud(int CategoryId);
    }
}
