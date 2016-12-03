using BizMall.Data.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMall.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IRepositoryUser _repositoryUser;
        private readonly IRepositoryCompany _repositoryCompany;

        public AdminController(IRepositoryUser repositoryUser, IRepositoryCompany repositoryCompany)
        {
            _repositoryUser = repositoryUser;
            _repositoryCompany = repositoryCompany;
        }

        [HttpGet]
        public IActionResult AdminPanel()
        {
            var currentUser = _repositoryUser.GetCurrentUser(User.Identity.Name);
            if (currentUser != null)
            {
                var company = _repositoryCompany.GetUserCompany(currentUser);
                ViewData["Company"] = company;
            }
            else
            {
                Redirect("/");
            }

            return View();
        }
    }
}
