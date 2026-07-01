using System.ComponentModel.DataAnnotations;

namespace CinemaBooking.Models.ViewModels
{
    public class MovieFormViewModel
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Range(1, 600)]
        [Display(Name = "Duration (minutes)")]
        public int DurationMinutes { get; set; }

        public string? ExistingPosterPath { get; set; }

        [Display(Name = "Poster Image")]
        public IFormFile? PosterFile { get; set; }

        [Display(Name = "Categories")]
        public List<int> SelectedCategoryIds { get; set; } = new();

        public List<Category> AllCategories { get; set; } = new();
    }
}