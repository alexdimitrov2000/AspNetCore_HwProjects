namespace Eventures.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Eventures.Data;
    using Eventures.Models;

    public class EventsService : IEventsService
    {
        private readonly EventuresDbContext context;

        public EventsService(EventuresDbContext context)
        {
            this.context = context;
        }

        public async Task CreateAsync(string name, string place, decimal? ticketPrice, int? totalTickets, DateTime? start, DateTime? end)
        {
            var @event = new Event
            {
                Name = name,
                Place = place,
                TicketPrice = (decimal)ticketPrice,
                TotalTickets = (int)totalTickets,
                Start = (DateTime)start,
                End = (DateTime)end,
            };

            this.context.Events.Add(@event);
            await this.context.SaveChangesAsync();
        }

        public List<Event> GetAllOrderedByStart()
        {
            return this.context.Events.OrderBy(e => e.Start).ToList();
        }

        public Event GetEventById(string id)
        {
            return this.context.Events.SingleOrDefault(e => e.Id == id);
        }

        public List<Order> GetMyEventsOrders(string customerId)
        {
            var orders = this.context.Orders.Where(o => o.CustomerId == customerId).ToList();
            return orders;
        }
    }
}
