using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BizMall.Data.DBContexts;
using BizMall.Data.Repositories.Abstract;
using BizMall.Models.CompanyModels;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using ArticleList.Models.CommonModels;

namespace BizMall.Data.Repositories.Concrete
{
    public class RepositoryKW : RepositoryBase, IRepository, IRepositoryKW
    {
        private CategoryType categoryType;
        int countOfSimilarArticlesOnArticlePage = 0;
        int pageSize = 0;
        int pageAdminSize = 0;
        int pageSearchSize = 0;

        public RepositoryKW(ApplicationDbContext ctx, IOptions<AppSettings> settings) : base(ctx)
        {
            categoryType = settings.Value.CategoryType;

            countOfSimilarArticlesOnArticlePage = Convert.ToInt32(settings.Value.CountOfSimilarArticlesOnArticlePage);
            pageSize = Convert.ToInt32(settings.Value.PageSize);
            pageAdminSize = Convert.ToInt32(settings.Value.PageAdminSize);
            pageSearchSize = Convert.ToInt32(settings.Value.PageSearchSize);
        }

        public IQueryable<KW> Kws(string CategoryId)
        {
            if (CategoryId == null)
                return _ctx.KWs.Where(kw => kw.CategoryType == categoryType && kw.Category == null)
                            .Include(kw => kw.Category)
                            .Include(kw => kw.Category.ParentCategory)
                            .OrderBy(kw => kw.kw);
            else
                return _ctx.KWs.Where(kw => kw.Category.Id == Convert.ToInt32(CategoryId))
                                .Include(kw => kw.Category)
                                .Include(kw => kw.Category.ParentCategory)
                                .OrderBy(kw => kw.kw);
        }

        public IQueryable<KW> Kws(string Category, int page, out PagingInfo pagingInfo)
        {
            int totaItems = _ctx.KWs
                .Where(kw => kw.CategoryType == categoryType && (Category == null || kw.Category.EnTitle == Category))
                .Count();

            pagingInfo = new PagingInfo
            {
                CategoryEnTitleForLink = Category,
                CurrentPage = page,
                ItemsPerPage = pageAdminSize,
                TotalItems = totaItems
            };
            
            return _ctx.KWs.Where(kw => kw.CategoryType == categoryType&&(Category == null || kw.Category.EnTitle == Category))
                            .Include(kw => kw.Category)
                            .Include(kw => kw.Category.ParentCategory)
                            .OrderBy(kw => kw.kw)
                            .Skip((page - 1) * pageAdminSize)
                            .Take(pageAdminSize); 
        }

        public KW SaveKW(KW model)
        {
            //Редактирование СУЩЕСТВУЮЩЕЙ позиции (дата UpdateStatus не меняется - она меняется только из списка неактивных товаров)
            if (model.Id != 0)
            {
                var dbEntry = _ctx.KWs.Where(c => c.Id == model.Id).SingleOrDefault();

                dbEntry.CategoryType = categoryType;
                dbEntry.kw = model.kw;
                dbEntry.CategoryId = model.CategoryId;

                _ctx.Entry(dbEntry).State = EntityState.Modified;
                _ctx.SaveChanges();
            }
            //Добавление НОВОЙ позиции (в т.ч. дата UpdateStatus выставляется на текущий день - берется из параметра - good)
            else
            {
                model.CategoryType = categoryType;
                _ctx.KWs.Add(model);
                _ctx.SaveChanges();
            }
           
            return model;
        }

        public void DeleteKW(int itemId)
        {
            try
            {
                var item = _ctx.KWs
                            .Where(c => c.Id == itemId)
                            .FirstOrDefault();
                    _ctx.Entry(item).State = EntityState.Deleted;
                    _ctx.SaveChanges();

            }
            catch (Exception ex)
            {
                var tmp = 0;
            }
        }

        public KW GetKwById(int id)
        {
            var item = _ctx.KWs.Where(kw => kw.Id == id)                           
                        .Include(kw => kw.Category)
                        .Include(kw => kw.Category.ParentCategory)
                        .FirstOrDefault();
            return item;
        }

        public IQueryable<KW> SearchStringKws(string searchstring, int page, out PagingInfo pagingInfo)
        {
            int totaItems = _ctx.KWs
                .Where(kw => kw.CategoryType == categoryType && kw.kw.Contains(searchstring))
                .Count();

            pagingInfo = new PagingInfo
            {
                searchstring = searchstring,
                CurrentPage = page,
                ItemsPerPage = pageAdminSize,
                TotalItems = totaItems
            };

            return _ctx.KWs
                        .Where(kw => kw.CategoryType == categoryType && kw.kw.Contains(searchstring))
                        .Include(kw => kw.Category)
                        .Include(kw => kw.Category.ParentCategory)
                        .OrderBy(kw => kw.kw).AsQueryable()
                        .Skip((page - 1) * pageAdminSize)
                        .Take(pageAdminSize); ;
        }

        public IQueryable<KW> KwsForTagCloud(int CategoryId)
        {
            if (CategoryId == 0)
            {
                var sitekws = _ctx.KWs.Where(kw => kw.CategoryType == categoryType && kw.CategoryId == null)
                                .Include(kw => kw.Category)
                                .Include(kw => kw.Category.ParentCategory)
                                .OrderBy(kw => kw.kw)
                                .Take(15);

                return sitekws;
            }
            else
            {
                var catkws = _ctx.KWs.Where(kw => kw.CategoryId == CategoryId)
                .Include(kw => kw.Category)
                .Include(kw => kw.Category.ParentCategory)
                .OrderBy(kw => kw.kw)
                .Take(10);

                var sitekws = _ctx.KWs.Where(kw => kw.CategoryType == categoryType && kw.CategoryId == null)
                            .Include(kw => kw.Category)
                            .Include(kw => kw.Category.ParentCategory)
                            .OrderBy(kw => kw.kw)
                            .Take(5);
                List<KW> kws = new List<KW>(catkws.ToList());
                kws.AddRange(sitekws.ToList());
                return kws.AsQueryable<KW>();
            }
        }
    }
}
