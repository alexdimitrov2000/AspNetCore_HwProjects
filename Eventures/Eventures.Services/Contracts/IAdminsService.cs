using Eventures.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eventures.Services.Contracts
{
    public interface IAdminsService
    {
        Task<IList<User>> GetAdminUsers();

        ICollection<User> GetStandardUsers();

        Task<User> GetUserById(string id);

        Task PromoteUserToAdminById(string userId);

        Task DemoteAdminToRegularUser(User user);
    }
}
