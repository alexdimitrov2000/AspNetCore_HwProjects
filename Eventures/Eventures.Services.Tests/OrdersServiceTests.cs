using Eventures.Data;
using Eventures.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Eventures.Services.Tests
{
    public class OrdersServiceTests
    {
        public OrdersServiceTests()
        {
            this.Options = new DbContextOptionsBuilder<EventuresDbContext>()
                        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                        .Options;
        }

        public DbContextOptions<EventuresDbContext> Options { get; }

        [Fact]
        public async Task CreateMethod_ShouldAddOrderToDbContext()
        {
            var context = new EventuresDbContext(this.Options);

            var ordersService = new OrdersService(context);

            await ordersService.CreateAsync(new Event(), 10, new User());

            Assert.Equal(1, context.Orders.Count());
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllOrdersWithLoadedEventAndCustomer()
        {
            var context = new EventuresDbContext(this.Options);

            var @event = new Event { TotalTickets = 100, Name = "UnitTesting" };
            var customer = new User { FirstName = "Pesho" };

            var orders = new List<Order>
            {
                new Order{Event = @event, TicketsCount = 5, Customer = customer},
                new Order{Event = @event, TicketsCount = 5, Customer = customer},
                new Order{Event = @event, TicketsCount = 5, Customer = customer},
                new Order{Event = @event, TicketsCount = 5, Customer = customer},
                new Order{Event = @event, TicketsCount = 5, Customer = customer},
            };

            await context.Orders.AddRangeAsync(orders);
            await context.SaveChangesAsync();

            var ordersService = new OrdersService(context);

            var resultOrders = ordersService.GetAll();

            Assert.Equal(5, resultOrders.Count);
            Assert.Equal(customer.FirstName, resultOrders[0].Customer.FirstName);
            Assert.Equal(@event.Name, resultOrders[0].Event.Name);
        }

        [Fact]
        public async Task GetAll_WithNoOrdersInDbShouldReturnEmptyList()
        {
            var context = new EventuresDbContext(this.Options);

            var orders = new List<Order>
            {
            };

            await context.Orders.AddRangeAsync(orders);
            await context.SaveChangesAsync();

            var ordersService = new OrdersService(context);

            var resultOrders = ordersService.GetAll();

            Assert.Empty(resultOrders);
        }
    }
}
