using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BizMall.Models.CompanyModels
{
    public enum AccountType{ Company=1, PrivatePerson=2}
    public class Company
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Telephone { get; set; }
        public string ContactEmail { get; set; }//при создании аккаунта - он равен емэйлу пользователя - потом в ЛК в профиле - его можно поменять
        public AccountType AccountType{ get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser User { get; set; }
        public List<RelCompanyGood> Goods { get; set; }
        public List<RelCompanyImage> Images { get; set; }
        public Company()
        {
            Goods = new List<RelCompanyGood>();
            Images = new List<RelCompanyImage>();
        }
    }
}
