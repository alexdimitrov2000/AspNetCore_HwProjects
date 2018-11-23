using Eventures.Services.Contracts;
using Eventures.Web.CustomFIlters;
using Eventures.Web.Models.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Eventures.Web.Controllers
{
    public class EventsController : Controller
    {
        private readonly ILogger<EventsController> logger;
        private readonly IEventsService eventsService;

        public EventsController(ILogger<EventsController> logger, IEventsService eventsService)
        {
            this.logger = logger;
            this.eventsService = eventsService;
        }

        [Authorize]
        public IActionResult All()
        {
            var events = this.eventsService.GetAllOrderedByStart()
                .Select(e => new EventViewModel
                {
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
