using System.ComponentModel.DataAnnotations;

namespace CinemaBooking.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Range(1, 600)]
        public int DurationMinutes { get; set; }

        public string? PosterPath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<MovieCategory> MovieCategories { get; set; } = new List<MovieCategory>();
        public ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
    }
}