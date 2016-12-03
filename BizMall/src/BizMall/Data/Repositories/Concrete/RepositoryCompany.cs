using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BizMall.Models;
using BizMall.Models.CompanyModels;
using BizMall.Data.DBContexts;
using BizMall.Data.Repositories.Abstract;
using BizMall.Data.Repositories.Concrete;
using Microsoft.EntityFrameworkCore;
using BizMall.ViewModels.AccountViewModels;

namespace BizMall.Data.Repositories.Concrete
{
    //Сам context наследуем из RepositoryBase
    public class RepositoryCompany : RepositoryBase, IRepository, IRepositoryCompany
    {
        public RepositoryCompany(ApplicationDbContext ctx) : base(ctx)
        {
        }

        public Company SaveCompanyAccount(Company company)
        {
            if (company.Id != 0)
            {
                _ctx.Entry(company).State = EntityState.Modified;
                _ctx.SaveChanges();
                return company;
            }
            else
            {
                _ctx.Companies.Add(company);
                _ctx.SaveChanges();
                return company;
            }
        }

        public Company CreateDefaultUserCompany(string userId)
        {
            var company = new Company { Title = "Моя компания", ApplicationUserId = userId};
            _ctx.Companies.Add(company);
            _ctx.SaveChanges();
            return company;
        }

        public IEnumerable<Company> GetAllCompanies()
        {
            throw new NotImplementedException();
        }

        public Company GetCompany(int shopId)
        {
            throw new NotImplementedException();
        }

        public Company GetUserCompany(ApplicationUser User)
        {
            var company = _ctx.Companies
                                .Where(c => c.ApplicationUserId == User.Id)
                                .Include(c => c.Images).ThenInclude(i => i.Image)
                                .Include(c => c.Goods)
                                .FirstOrDefault();
            return company;
        }

        public Company GetUserCompanyWithNoTrackingImages(ApplicationUser User)
        {
            var company = _ctx.Companies
                                .Where(c => c.ApplicationUserId == User.Id)
                                .Include(c => c.Images)
                                .AsNoTracking()
                                .FirstOrDefault();
            return company;
        }
    }
}
