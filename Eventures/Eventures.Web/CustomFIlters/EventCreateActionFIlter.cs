using Eventures.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Eventures.Web.CustomFIlters
{
    public class EventCreateActionFIlter : ActionFilterAttribute
    {
        private readonly EventuresDbContext context;
        private readonly ILogger<EventCreateActionFIlter> logger;

        public EventCreateActionFIlter(EventuresDbContext context, ILogger<EventCreateActionFIlter> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (!context.ModelState.IsValid)
            {
                return;
            }
            var @event = this.context.Events.Last();
            var userName = context.HttpContext.User.Identity.Name;
            this.logger.LogInformation($"[{DateTime.UtcNow.ToShortDateString()}] Administrator {userName} create event {@event.Name} ({@event.Start.ToShortDateString()} / {@event.End.ToShortDateString()}).");
            base.OnActionExecuted(context);
        }
    }
}
