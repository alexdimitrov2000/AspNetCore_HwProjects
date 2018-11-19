using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Eventures.Web.Models.Events
{
    public class EventCreateViewModel
    {
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 10, ErrorMessage = "{0} must be at least 10 characters long.")]
        [RegularExpression(".")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} field is required and cannot be empty.")]
        [RegularExpression(".")]
        [StringLength(int.MaxValue, MinimumLength = 1, ErrorMessage = "{0} field is required and cannot be empty.")]
        public string Place { get; set; }

        [Required(ErrorMessage = "{0} must be a valid Date")]
        [DataType(DataType.Date, ErrorMessage = "{0} must be a valid Date")]
        public DateTime Start { get; set; }

        [Required]
        [DataType(DataType.DateTime, ErrorMessage = "{0} must be a valid Date")]
        public DateTime End { get; set; }

        [RegularExpression("^-?[1-9]\\d*$", ErrorMessage = "{0} field cannot be zero.")]
        [Display(Name = "Total Tickets")]
        [Required]
        public int TotalTickets { get; set; }

        [Required]
        [Display(Name = "Price per Ticket")]
        [Range(0, (double)decimal.MaxValue)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TicketPrice { get; set; }
    }
}
