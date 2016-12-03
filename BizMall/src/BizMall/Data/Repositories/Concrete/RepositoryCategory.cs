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
    }
}
