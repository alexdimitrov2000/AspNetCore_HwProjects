using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Chushka.App.Models;
using Chushka.App.Models.Home;
using System.Linq;

namespace Chushka.App.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var products = this.Context.Products
                    .Select(p => new IndexProductViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        Description = p.Description
                    })
                    .ToList();

                var productCollection = new ProductCollectionViewModel
                {
                    Products = products
                };

                return this.View(productCollection);
            }

            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
