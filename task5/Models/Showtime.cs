using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaBooking.Models
{
    public class Showtime
    {
        public int Id { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        public int HallId { get; set; }
        public Hall Hall { get; set; } = null!;

        [Required]
        public DateTime StartTime { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        [Range(0.01, 10000)]
        public decimal TicketPrice { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        // Helper: seats currently taken by confirmed bookings (computed in service/repo, not mapped)
        [NotMapped]
        public int SeatsBooked => Bookings?.Where(b => b.Status == BookingStatus.Confirmed).Sum(b => b.SeatsCount) ?? 0;

        [NotMapped]
        public int SeatsAvailable => Hall != null ? Hall.SeatCapacity - SeatsBooked : 0;
    }
}