namespace Eventures.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Eventures.Data;
    using Eventures.Models;
    using Microsoft.AspNetCore.Identity;

    public class AdminsService : IAdminsService
    {
        private readonly EventuresDbContext context;
        private readonly UserManager<User> userManager;

        public AdminsService(EventuresDbContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IList<User>> GetAdminUsers()
        {
            return await this.userManager.GetUsersInRoleAsync("Admin");
        }

        public ICollection<User> GetStandardUsers()
        {
            return this.userManager.Users.Where(u => !this.GetAdminUsers().GetAwaiter().GetResult().Contains(u)).ToList();
        }

        public async Task<User> GetUserById(string id)
        {
            return await this.userManager.FindByIdAsync(id);
        }

        public async Task PromoteUserToAdminById(string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);

            await this.userManager.AddToRoleAsync(user, "Admin");
        }

        public async Task DemoteAdminToRegularUser(User user)
        {
            await this.userManager.RemoveFromRoleAsync(user, "Admin");
        }
    }
}
