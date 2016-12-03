using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BizMall.Models;
using BizMall.Models.CompanyModels;


namespace BizMall.Data.Repositories.Abstract
{
    public interface IRepositoryUser
    {
        ApplicationUser GetCurrentUser(string UserEmail);
    }
}
