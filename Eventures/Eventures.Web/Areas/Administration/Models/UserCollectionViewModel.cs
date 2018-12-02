using System.Collections.Generic;

namespace Eventures.Web.Areas.Administration.Models
{
    public class UserCollectionViewModel
    {
        public List<UserViewModel> AdminUsers { get; set; }

        public List<UserViewModel> NonAdminUsers { get; set; }
    }
}
