using Chushka.App.Data;
using Microsoft.AspNetCore.Mvc;

namespace Chushka.App.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
            this.Context = new ChushkaDbContext();
        }

        public ChushkaDbContext Context { get; set; }
    }
}
