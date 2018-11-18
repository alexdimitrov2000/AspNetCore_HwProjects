using Eventures.Data;
using Microsoft.AspNetCore.Mvc;

namespace Eventures.Web.Controllers
{
    public class BaseController : Controller
    {
        public BaseController(EventuresDbContext context)
        {
            this.Context = context;
        }

        public EventuresDbContext Context { get; set; }
    }
}