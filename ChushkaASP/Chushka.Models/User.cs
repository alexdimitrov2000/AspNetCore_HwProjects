namespace Chushka.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;

    public class User : IdentityUser<int>
    {
        public string FullName { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
