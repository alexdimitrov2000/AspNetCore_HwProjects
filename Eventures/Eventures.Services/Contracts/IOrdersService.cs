using Eventures.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eventures.Services.Contracts
{
    public interface IOrdersService
    {
        Task CreateAsync(Event @event, int tickets, User customer);

        List<Order> GetAll();
    }
}
