using System.ComponentModel.DataAnnotations;

namespace CinemaBooking.Models.ViewModels
{
    public class ShowtimeFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Movie")]
        public int MovieId { get; set; }

        [Required]
        [Display(Name = "Hall")]
        public int HallId { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; } = DateTime.Now.AddDays(1);

        [Range(0.01, 10000)]
        [Display(Name = "Ticket Price")]
        public decimal TicketPrice { get; set; }

        public List<Movie> AllMovies { get; set; } = new();
        public List<Hall> AllHalls { get; set; } = new();
    }
}