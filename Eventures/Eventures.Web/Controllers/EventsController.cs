using AutoMapper;
using Eventures.Models;
using Eventures.Services.Contracts;
using Eventures.Web.CustomFIlters;
using Eventures.Web.Models.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Eventures.Web.Controllers
{
    public class EventsController : Controller
    {
        private const int NumberOfEntitiesOnPage = GlobalConstants.NumberOfEntitiesOnPage;

        private readonly ILogger<EventsController> logger;
        private readonly UserManager<User> userManager;
        private readonly IEventsService eventsService;
        private readonly IOrdersService ordersService;
        private readonly IMapper mapper;

        public EventsController(ILogger<EventsController> logger,
                                UserManager<User> userManager,
                                IEventsService eventsService,
                                IOrdersService ordersService,
                                IMapper mapper)
        {
            this.logger = logger;
            this.userManager = userManager;
            this.eventsService = eventsService;
            this.ordersService = ordersService;
            this.mapper = mapper;
        }

        [Authorize]
        public IActionResult All(int page = 1)
        {
            var events = this.eventsService.GetAllOrderedByStart()
               .Where(e => e.TotalTickets > 0)
               .Select(e => this.mapper.Map<EventViewModel>(e));
            
            var validatedPage = this.ValidatePage(page, events.Count());
            if (validatedPage != page)
                return this.Redirect($"/Events/All?page={validatedPage}");

            this.ViewData["Page"] = page;
            this.ViewData["HasNextPage"] = ((page + 1) * NumberOfEntitiesOnPage) - NumberOfEntitiesOnPage < events.Count();

            return this.View(new EventCollectionViewModel
            {
                Events = events
                           .Skip((page - 1) * NumberOfEntitiesOnPage)
                           .Take(NumberOfEntitiesOnPage)
                           .ToList()
            });
        }

        [Authorize]
        public IActionResult MyEvents(int page = 1)
        {
            var customerId = this.userManager.GetUserId(this.User);
            var myEvents = this.eventsService.GetMyEventsOrders(customerId)
                .OrderBy(o => o.Event.Start)
                .Select(o => this.mapper.Map<MyEventViewModel>(o));

            var validatedPage = this.ValidatePage(page, myEvents.Count());
            if (validatedPage != page)
                return this.Redirect($"/Events/MyEvents?page={validatedPage}");

            this.ViewData["Page"] = page;
            this.ViewData["HasNextPage"] = ((page + 1) * NumberOfEntitiesOnPage) - NumberOfEntitiesOnPage < myEvents.Count();

            return this.View(new MyEventsCollectionViewModel
            {
                MyEvents = myEvents
                           .Skip((page - 1) * NumberOfEntitiesOnPage)
                           .Take(NumberOfEntitiesOnPage)
                           .ToList()
            });
        }

        [Authorize]
        public async Task<IActionResult> Order(string id, int tickets)
        {
            var @event = this.eventsService.GetEventById(id);
            var customer = this.userManager.GetUserAsync(this.User).Result;

            if (@event.TotalTickets < tickets)
            {
                this.TempData["Error"] = $"You cannot order {tickets} tickets for this event. There are only {@event.TotalTickets} tickets left.";
                this.TempData["EventId"] = @event.Id;
                return this.RedirectToAction("All", "Events", tickets);
            }

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
