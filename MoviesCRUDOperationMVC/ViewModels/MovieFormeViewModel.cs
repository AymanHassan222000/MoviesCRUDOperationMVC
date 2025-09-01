using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MoviesCRUDOperationMVC.CustomValidation;
using MoviesCRUDOperationMVC.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace MoviesCRUDOperationMVC.ViewModels
{
    public class MovieFormeViewModel
    {
        public int Id { get; set; }

        [Required, MinLength(3), StringLength(250)]
        public string Title { get; set; }

        [ValidYear(1950)]
        public int Year { get; set; }

        [Range(1,10)]
        public float? Rate { get; set; }

        [Required, MinLength(20), MaxLength(2500)]
        public string StoryLine { get; set; }

        public IFormFile? Poster { get; set; }

        [Display(Name = "Genre Name")]
        public byte GenreId { get; set; }

        public IEnumerable<Genre>? Genres { get; set; }

    }
}
