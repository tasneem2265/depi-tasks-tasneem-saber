using System.ComponentModel.DataAnnotations;

namespace CinemaBooking.Models
{
    public class Cinema
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(250)]
        public string Address { get; set; } = string.Empty;

        public ICollection<Hall> Halls { get; set; } = new List<Hall>();
    }
}