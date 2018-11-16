using Chushka.App.Models.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Chushka.App.Controllers
{
    public class OrdersController : BaseController
    {
        [Authorize]
        [Route("Orders/All")]
        public IActionResult All()
        {
            var orders = this.Context.Orders
                .Select(o => new OrderViewModel
                {
                    Id = o.Id,
                    CustomerName = o.Client.FullName,
                    ProductName = o.Product.Name,
                    OrderedOn = o.OrderedOn.ToString("hh:mm dd/MM/yyyy")
                })
                .ToList();

            var orderCollection = new OrderCollectionViewModel
            {
                Orders = orders
            };

            return this.View(orderCollection);
        }
    }
}
