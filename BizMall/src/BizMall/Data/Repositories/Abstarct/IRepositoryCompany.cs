using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BizMall.Models;
using BizMall.Models.CompanyModels;
using BizMall.ViewModels.AccountViewModels;

namespace BizMall.Data.Repositories.Abstract
{
    public interface IRepositoryCompany
    {
        IEnumerable<Company> GetAllCompanies();
        Company GetUserCompany(ApplicationUser currentUser);
        Company GetUserCompanyWithNoTrackingImages(ApplicationUser currentUser);
        Company GetCompany(int companyId);
        Company SaveCompanyAccount(Company company);
    }
}
