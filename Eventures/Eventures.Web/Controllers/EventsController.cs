using Eventures.Models;
using Eventures.Services.Contracts;
using Eventures.Web.CustomFIlters;
using Eventures.Web.Models.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Eventures.Web.Controllers
{
    public class EventsController : Controller
    {
        private readonly ILogger<EventsController> logger;
        private readonly UserManager<User> userManager;
        private readonly IEventsService eventsService;
        private readonly IOrdersService ordersService;

        public EventsController(ILogger<EventsController> logger, UserManager<User> userManager, IEventsService eventsService, IOrdersService ordersService)
        {
            this.logger = logger;
            this.userManager = userManager;
            this.eventsService = eventsService;
            this.ordersService = ordersService;
        }

        [Authorize]
        public IActionResult All()
        {
            var events = this.eventsService.GetAllOrderedByStart()
                .Select(e => new EventViewModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    Start = e.Start.ToString("dd-MMM-yy hh:mm:ss"),
                    End = e.End.ToString("dd-MMM-yy hh:mm:ss"),
                    Place = e.Place
                })
                .ToList();

            return this.View(new EventCollectionViewModel
            {
                Events = events
            });
        }

        [Authorize]
        public IActionResult MyEvents()
        {
            var customerId = this.userManager.GetUserId(this.User);
            var myEvents = this.eventsService.GetMyEventsOrders(customerId)
                .OrderBy(o => o.Event.Start)
                .Select(o => new MyEventViewModel
                {
                    Name = o.Event.Name,
                    Start = o.Event.Start.ToString("dd-MMM-yy hh:mm:ss"),
                    End = o.Event.End.ToString("dd-MMM-yy hh:mm:ss"),
                    Tickets = o.TicketsCount
                })
                .ToList();

            return this.View(new MyEventsCollectionViewModel
            {
                MyEvents = myEvents
            });
        }

        [Authorize]
        public async Task<IActionResult> Order(string id, int tickets)
        {
            var @event = this.eventsService.GetEventById(id);
            var customer = this.userManager.GetUserAsync(this.User).Result;

            await this.ordersService.CreateAsync(@event, tickets, customer);

            return this.Redirect("/Events/MyEvents");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [TypeFilter(typeof(EventCreateActionFIlter))]
        public async Task<IActionResult> Create(EventCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            await this.eventsService.CreateAsync(model.Name, model.Place, model.TicketPrice, model.TotalTickets, model.Start, model.End);

            this.logger.LogInformation("Event created: " + model.Name, model);

            return this.Redirect("/Events/All");
        }
    }
}
