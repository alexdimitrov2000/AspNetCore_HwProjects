using Eventures.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eventures.Services.Contracts
{
    public interface IEventsService
    {
        Task CreateAsync(string name, string place, decimal? ticketPrice, int? totalTickets, DateTime? start, DateTime? end);

        List<Event> GetAllOrderedByStart();
    }
}
