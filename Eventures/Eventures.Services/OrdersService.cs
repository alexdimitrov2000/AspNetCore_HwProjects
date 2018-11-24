namespace Eventures.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Eventures.Data;
    using Eventures.Models;
    using Microsoft.EntityFrameworkCore;

    public class OrdersService : IOrdersService
    {
        private readonly EventuresDbContext context;

        public OrdersService(EventuresDbContext context)
        {
            this.context = context;
        }
        public async Task CreateAsync(Event @event, int tickets, User customer)
        {
            var order = new Order
            {
                OrderedOn = DateTime.UtcNow,
                TicketsCount = tickets,
                Event = @event,
                Customer = customer
            };

            await this.context.Orders.AddAsync(order);
            await this.context.SaveChangesAsync();
        }

        public List<Order> GetAll()
        {
            return this.context.Orders.Include(o => o.Customer).Include(o => o.Event).ToList();
        }
    }
}
