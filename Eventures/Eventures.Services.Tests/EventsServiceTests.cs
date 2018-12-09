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
    public class EventsServiceTests
    {
        public EventsServiceTests()
        {
            this.Options = new DbContextOptionsBuilder<EventuresDbContext>()
                        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                        .Options;
        }

        public DbContextOptions<EventuresDbContext> Options { get; }

        [Fact]
        public async Task CreateMethod_ShouldAddEventToContext()
        {
            var context = new EventuresDbContext(this.Options);

            var eventsService = new EventsService(context);
            await eventsService.CreateAsync("Name", "Place", 10.00m, 100, DateTime.UtcNow, DateTime.UtcNow.AddDays(3));

            Assert.Equal(1, context.Events.Count());
        }

        [Fact]
        public async Task GetAllOrderedByStart_ShouldReturnOrderedByStartList()
        {
            var context = new EventuresDbContext(this.Options);

            var events = new List<Event>
            {
                new Event
                {
                    Start = DateTime.UtcNow.AddDays(1)
                },
                new Event
                {
                    Start = DateTime.UtcNow.AddDays(3)
                },
                new Event
                {
                    Start = DateTime.UtcNow.AddDays(2)
                },
            };
            await context.Events.AddRangeAsync(events);
            await context.SaveChangesAsync();

            var eventsService = new EventsService(context);

            var serviceResult = eventsService.GetAllOrderedByStart();

            var expected = events.OrderBy(e => e.Start);

            Assert.Equal(expected, serviceResult);
        }

        [Fact]
        public async Task GetAllOrderedByStart_EmptyDbSetShouldReturnEmptyList()
        {
            var context = new EventuresDbContext(this.Options);

            var events = new List<Event>
            {
            };
            await context.Events.AddRangeAsync(events);
            await context.SaveChangesAsync();

            var eventsService = new EventsService(context);

            var serviceResult = eventsService.GetAllOrderedByStart();

            var expected = events.OrderBy(e => e.Start);

            Assert.Equal(expected, serviceResult);
        }

        [Fact]
        public void GetEventById_ShouldReturnSingleEventWithTheGivenId()
        {
            var context = new EventuresDbContext(this.Options);
            
            var events = new List<Event>
            {
                new Event { Id = "1", Name = "Event1" },
                new Event { Id = "2", Name = "Event2" },
                new Event { Id = "3", Name = "Event3" },
            };
            context.Events.AddRange(events);
            context.SaveChanges();

            var service = new EventsService(context);

            var resultEvent = service.GetEventById("1");

            Assert.Equal(events[0], resultEvent);
            Assert.Equal(events[0].Name, resultEvent.Name);
        }

        [Theory]
        [InlineData("2")]
        [InlineData("")]
        [InlineData("asd-asdasd-asd-asd")]
        [InlineData(null)]
        public void GetEventById_WithInexistingIdShouldReturnNull(string id)
        {
            var context = new EventuresDbContext(this.Options);

            var @event = new Event { Id = "1" };
            context.Events.Add(@event);
            context.SaveChanges();

            var service = new EventsService(context);

            var resultEvent = service.GetEventById(id);

            Assert.Null(resultEvent);
        }

        [Fact]
        public void GetMyEventsOrders_ShouldReturnListOfMyOrders()
        {
            var orders = new List<Order>
            {
                new Order
                {
                    CustomerId = "1"
                },
                new Order
                {
                    CustomerId = "1"
                },
                new Order
                {
                    CustomerId = "1"
                },
                new Order
                {
                    CustomerId = "2"
                },
                new Order
                {
                    CustomerId = "2"
                },
                new Order
                {
                    CustomerId = "2"
                },
            };

            var context = new EventuresDbContext(this.Options);
            context.Orders.AddRange(orders);
            context.SaveChanges();

            var service = new EventsService(context);

            var resultList = service.GetMyEventsOrders("1");

            Assert.Equal(3, resultList.Count());
        }
        
        [Theory]
        [InlineData("3")]
        [InlineData("")]
        [InlineData("asd-asdasd-asd-asd")]
        [InlineData(null)]
        public void GetMyEventsOrders_WithNoOrdersInContextShouldReturnEmptyList(string customerId)
        {
            var orders = new List<Order>
            {
                new Order
                {
                    CustomerId = "1"
                },
                new Order
                {
                    CustomerId = "1"
                },
                new Order
                {
                    CustomerId = "1"
                },
                new Order
                {
                    CustomerId = "2"
                },
                new Order
                {
                    CustomerId = "2"
                },
                new Order
                {
                    CustomerId = "2"
                },
            };

            var context = new EventuresDbContext(this.Options);
            context.Orders.AddRange(orders);
            context.SaveChanges();

            var service = new EventsService(context);

            var resultList = service.GetMyEventsOrders(customerId);

            Assert.Empty(resultList);
        }
    }
}
