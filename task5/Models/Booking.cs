using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaBooking.Models
{
    public enum BookingStatus
    {
        Confirmed = 0,
        Cancelled = 1
    }

    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public int ShowtimeId { get; set; }
        public Showtime Showtime { get; set; } = null!;

        [Range(1, 50)]
        public int SeatsCount { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        public decimal TotalPrice { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        public BookingStatus Status { get; set; } = BookingStatus.Confirmed;
    }
}