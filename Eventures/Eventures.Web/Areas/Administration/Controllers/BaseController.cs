using Eventures.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Eventures.Web.Areas.Administration.Controllers
{
    public class BaseController : Controller
    {
        public ViewResult Error(string errorMessage)
        {
            return this.View("Error", errorMessage);
        }
    }
}
