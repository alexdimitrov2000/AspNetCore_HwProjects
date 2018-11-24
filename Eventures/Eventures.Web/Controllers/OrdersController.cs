using Eventures.Services.Contracts;
using Eventures.Web.Models.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Eventures.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrdersService ordersService;

        public OrdersController(IOrdersService ordersService)
        {
            this.ordersService = ordersService;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult All()
        {
            var orders = this.ordersService.GetAll()
                .Select(o => new OrderViewModel
                {
                    EventName = o.Event.Name,
                    CustomerName = o.Customer.UserName,
                    OrderedOn = o.OrderedOn.ToString("dd-MMM-yy hh:mm:ss")
                })
                .OrderBy(o => o.OrderedOn)
                .ToList();

            return this.View(new OrderCollectionViewModel
            {
                Orders = orders
            });
        }
    }
}
