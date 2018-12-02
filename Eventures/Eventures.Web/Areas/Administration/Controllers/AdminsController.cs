using AutoMapper;
using Eventures.Services.Contracts;
using Eventures.Web.Areas.Administration.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Eventures.Web.Areas.Administration.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Administration")]
    public class AdminsController : BaseController
    {
        private readonly IAdminsService adminsService;
        private readonly IMapper mapper;

        public AdminsController(IAdminsService adminsService, IMapper mapper)
        {
            this.adminsService = adminsService;
            this.mapper = mapper;
        }

        public IActionResult Index()
        {
            var adminUsers = this.adminsService.GetAdminUsers().GetAwaiter().GetResult()
                .Select(u => this.mapper.Map<UserViewModel>(u))
                .ToList();

            var standardUsers = this.adminsService.GetStandardUsers()
                .Select(u => this.mapper.Map<UserViewModel>(u))
                .ToList();

            return View(new UserCollectionViewModel
            {
                AdminUsers = adminUsers,
                NonAdminUsers = standardUsers
            });
        }

        public IActionResult Promote(int page = 1)
        {
            var standardUsers = this.adminsService.GetStandardUsers()
                .Select(u => this.mapper.Map<UserViewModel>(u));

            var validatedPage = this.ValidatePage(page, standardUsers.Count());
            if (validatedPage != page)
                return this.Redirect($"/Admins/Promote?page={validatedPage}");

            this.ViewData["Page"] = page;
            this.ViewData["HasNextPage"] = ((page + 1) * GlobalConstants.NumberOfEntitiesOnPage) - GlobalConstants.NumberOfEntitiesOnPage < standardUsers.Count();
            this.ViewData["PageEntities"] = GlobalConstants.NumberOfEntitiesOnPage;

            return View(new UserCollectionViewModel
            {
                NonAdminUsers = standardUsers
                           .Skip((page - 1) * GlobalConstants.NumberOfEntitiesOnPage)
                           .Take(GlobalConstants.NumberOfEntitiesOnPage)
                           .ToList()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Promote(string id)
        {
            await this.adminsService.PromoteUserToAdminById(id);

            return this.RedirectToAction("Index", "Home", new { area = "" });
        }

        public IActionResult Demote(int page = 1)
        {
            var adminUsers = this.adminsService.GetAdminUsers().GetAwaiter().GetResult()
                .Select(u => this.mapper.Map<UserViewModel>(u));

            var validatedPage = this.ValidatePage(page, adminUsers.Count());
            if (validatedPage != page)
                return this.Redirect($"/Admins/Demote?page={validatedPage}");

            this.ViewData["Page"] = page;
            this.ViewData["HasNextPage"] = ((page + 1) * GlobalConstants.NumberOfEntitiesOnPage) - GlobalConstants.NumberOfEntitiesOnPage < adminUsers.Count();
            this.ViewData["PageEntities"] = GlobalConstants.NumberOfEntitiesOnPage;

            return View(new UserCollectionViewModel
            {
                AdminUsers = adminUsers
                           .Skip((page - 1) * GlobalConstants.NumberOfEntitiesOnPage)
                           .Take(GlobalConstants.NumberOfEntitiesOnPage)
                           .ToList()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Demote(string id)
        {
            var user = await this.adminsService.GetUserById(id);
            if (user.UserName == this.User.Identity.Name)
            {
                return this.Error("You cannot demote yourself.");
            }

            await this.adminsService.DemoteAdminToRegularUser(user);

            return this.RedirectToAction("Index", "Home", new { area = "" });
        }

        private int ValidatePage(int page, int collectionCount)
        {
            if (page < 1)
                return 1;

            if ((page * GlobalConstants.NumberOfEntitiesOnPage) - GlobalConstants.NumberOfEntitiesOnPage > collectionCount)
            {
                if (collectionCount % GlobalConstants.NumberOfEntitiesOnPage != 0)
                {
                    page = (collectionCount / GlobalConstants.NumberOfEntitiesOnPage) + 1;

                    return page;
                }
            }

            return page;
        }
    }
}