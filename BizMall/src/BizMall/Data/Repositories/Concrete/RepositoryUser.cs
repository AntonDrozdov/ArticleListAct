
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BizMall.Data.Repositories.Abstract;
using BizMall.Data;
using BizMall.Data.DBContexts;
using BizMall.Data.Repositories.Concrete;
using BizMall.Models;

namespace BizMall.Data.Repositories.Concrete
{
    public class RepositoryUser: RepositoryBase, IRepository, IRepositoryUser
    {
        public RepositoryUser(ApplicationDbContext ctx) : base(ctx)
        {
        }

        public ApplicationUser GetCurrentUser(string UserEmail)
        {
            return _ctx.Users.Where(u => u.Email == UserEmail).FirstOrDefault();
        }
    }
}
