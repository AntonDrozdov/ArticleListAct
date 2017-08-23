using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace BizMall.Models.CompanyModels
{
    public enum CategoryType
    {
        Default = 0, BuisnessFace = 1, BuisnessDev = 2, UgleglavProm = 3, Cook = 4, Joke = 5, MusicText = 6, Congratulation = 7, ForFun=8, Congratulation2 = 9
    }
}
