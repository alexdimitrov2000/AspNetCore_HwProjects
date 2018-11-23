using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Eventures.Web.Models.Events
{
    public class EventCreateViewModel
    {
        [Required(ErrorMessage = "'{0}' field is required.")]
        [StringLength(int.MaxValue, MinimumLength = 10, ErrorMessage = "'{0}' must be at least 10 characters long.")]
        [RegularExpression(".+")]
        public string Name { get; set; }

        [RegularExpression(".+")]
        [Required(ErrorMessage = "'{0}' field is required and cannot be empty.")]
        [StringLength(int.MaxValue, MinimumLength = 1, ErrorMessage = "'{0}' field is required and cannot be empty.")]
        public string Place { get; set; }

        [Required(ErrorMessage = "'{0}' field must be a valid Date.")]
        [DataType(DataType.DateTime, ErrorMessage = "'{0}' field must be a valid Date.")]
        public DateTime? Start { get; set; }

        [Required(ErrorMessage = "'{0}' field must be a valid Date.")]
        [DataType(DataType.DateTime, ErrorMessage = "'{0}' field must be a valid Date.")]
        public DateTime? End { get; set; }

        [Required(ErrorMessage = "'{0}' field is required.")]
        [Display(Name = "Total Tickets")]
        [RegularExpression("^-?[1-9]\\d*$", ErrorMessage = "'{0}' field cannot be zero.")]
        public int? TotalTickets { get; set; }

        [Display(Name = "Price per Ticket")]
        [Range(0, (double)decimal.MaxValue)]
        [Column(TypeName = "decimal(18, 2)")]
        [Required(ErrorMessage = "'{0}' field is required.")]
        public decimal? TicketPrice { get; set; }
    }
}
