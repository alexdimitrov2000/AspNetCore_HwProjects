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
        private const int NumberOfEntitiesOnPage = GlobalConstants.NumberOfEntitiesOnPage;

        private readonly IOrdersService ordersService;
        private readonly IMapper mapper;

        public OrdersController(IOrdersService ordersService, IMapper mapper)
        {
            this.ordersService = ordersService;
            this.mapper = mapper;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult All(int page = 1)
        {
            var orders = this.ordersService.GetAll()
                .Select(o => this.mapper.Map<OrderViewModel>(o))
                .OrderBy(o => o.OrderedOn);

            var validatedPage = this.ValidatePage(page, orders.Count());
            if (validatedPage != page)
                return this.Redirect($"/Orders/All?page={validatedPage}");

            this.ViewData["Page"] = page;
            this.ViewData["HasNextPage"] = ((page + 1) * NumberOfEntitiesOnPage) - NumberOfEntitiesOnPage < orders.Count();
            this.ViewData["PageEntities"] = NumberOfEntitiesOnPage;

            return this.View(new OrderCollectionViewModel
            {
                Orders = orders
                           .Skip((page - 1) * NumberOfEntitiesOnPage)
                           .Take(NumberOfEntitiesOnPage)
                           .ToList()
            });
        }
        
        private int ValidatePage(int page, int collectionCount)
        {
            if (page < 1)
                return 1;

            if ((page * NumberOfEntitiesOnPage) - NumberOfEntitiesOnPage > collectionCount)
            {
                if (collectionCount % NumberOfEntitiesOnPage != 0)
                {
                    page = (collectionCount / NumberOfEntitiesOnPage) + 1;

                    return page;
                }
            }

            return page;
        }
    }
}
