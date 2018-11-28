namespace Eventures.Web.Controllers
{
    using Models.Orders;
    using Services.Contracts;

    using AutoMapper;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class OrdersController : Controller
    {
        private readonly IOrdersService ordersService;
        private readonly IMapper mapper;

        public OrdersController(IOrdersService ordersService, IMapper mapper)
        {
            this.ordersService = ordersService;
            this.mapper = mapper;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult All()
        {
            var orders = this.ordersService.GetAll()
                .Select(o => this.mapper.Map<OrderViewModel>(o))
                .OrderBy(o => o.OrderedOn)
                .ToList();

            return this.View(new OrderCollectionViewModel
            {
                Orders = orders
            });
        }
    }
}
