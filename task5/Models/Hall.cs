using System.ComponentModel.DataAnnotations;

namespace CinemaBooking.Models
{
    public class Hall
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(1, 1000)]
        public int SeatCapacity { get; set; }

        public int CinemaId { get; set; }
        public Cinema Cinema { get; set; } = null!;

        public ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
    }
}