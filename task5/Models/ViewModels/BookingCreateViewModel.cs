using System.ComponentModel.DataAnnotations;

namespace CinemaBooking.Models.ViewModels
{
    public class BookingCreateViewModel
    {
        public int ShowtimeId { get; set; }

        public string MovieTitle { get; set; } = string.Empty;
        public string CinemaName { get; set; } = string.Empty;
        public string HallName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public decimal TicketPrice { get; set; }
        public int SeatsAvailable { get; set; }

        [Required]
        [Range(1, 50, ErrorMessage = "You must book between 1 and 50 seats.")]
        [Display(Name = "Number of Seats")]
        public int SeatsCount { get; set; } = 1;
    }
}