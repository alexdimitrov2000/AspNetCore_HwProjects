using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eventures.Models
{
    public class Event
    {
        public Event()
        {
            this.Orders = new List<Order>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Place { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public int TotalTickets { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TicketPrice { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
