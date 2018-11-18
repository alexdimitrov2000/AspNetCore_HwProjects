using Eventures.Data;
using Eventures.Models;
using Eventures.Web.CustomFIlters;
using Eventures.Web.Models.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Eventures.Web.Controllers
{
    public class EventsController : BaseController
    {
        private readonly ILogger<EventsController> logger;

        public EventsController(EventuresDbContext context, ILogger<EventsController> logger) : base(context)
        {
            this.logger = logger;
        }

        [Authorize]
        public IActionResult All()
        {
            var events = this.Context.Events
                .OrderBy(e => e.Start)
                .Select(e => new EventViewModel
                {
                    Name = e.Name,
                    Start = e.Start.ToString(),
                    End = e.End.ToString(),
                    Place = e.Place
                })
                .ToList();

            return this.View(new EventCollectionViewModel
            {
                Events = events
            });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [TypeFilter(typeof(EventCreateActionFIlter))]
        public IActionResult Create(EventCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            var @event = new Event
            {
                Name = model.Name,
                Place = model.Place,
                TicketPrice = (decimal)model.TicketPrice,
                TotalTickets = model.TotalTickets,
                Start = model.Start,
                End = model.End,
            };

            this.Context.Events.Add(@event);
            this.Context.SaveChanges();

            this.logger.LogInformation("Event created: " + @event.Name, @event);

            return this.Redirect("/Events/All");
        }
    }
}
