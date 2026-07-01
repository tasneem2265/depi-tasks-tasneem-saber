using System.ComponentModel.DataAnnotations;

namespace CinemaBooking.Models.ViewModels
{
    public class HallFormViewModel
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(1, 1000)]
        [Display(Name = "Seat Capacity")]
        public int SeatCapacity { get; set; }

        [Required]
        [Display(Name = "Cinema")]
        public int CinemaId { get; set; }

        public List<Cinema> AllCinemas { get; set; } = new();
    }
}