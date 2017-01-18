using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BizMall.Data.DBContexts;
using BizMall.Data.Repositories.Abstract;
using BizMall.Models.CompanyModels;

namespace BizMall.Data.Repositories.Concrete
{
    public class RepositoryCategory : RepositoryBase, IRepository, IRepositoryCategory
    {
        public RepositoryCategory(ApplicationDbContext ctx) : base(ctx)
        {
        }

        public IQueryable<Category> Categories()
        {
            return _ctx.Categories;
        }

        public List<string> SitemapCategories()
        {
            var Categories = _ctx.Categories;
            List<string> SitemapCategories = new List<string>();

            foreach(var topcat in Categories)
            {
                //определяем родительская она или нет
                if (topcat.CategoryId == null)
                {
                    bool IsParent = false;

                    foreach(var cat in Categories)
                    {
                        if(cat.CategoryId == topcat.Id)
                        {
                            IsParent = true;
                            break;
                        }
                    }

                    //если родительская то одно если нет то другое
                    if (IsParent == true)
                    {
                        foreach(var chcat in Categories)
                        {
                            if(chcat.CategoryId == topcat.Id)
                            {
                                SitemapCategories.Add(chcat.EnTitle);
                            }
                        }

                        }
                    if (IsParent == false)
                    {
                        SitemapCategories.Add(@topcat.EnTitle);                              
                    }
                }
            }

            return SitemapCategories;
        }
    }
}
