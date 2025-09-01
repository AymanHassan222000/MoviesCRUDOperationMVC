using MoviesCRUDOperationMVC.CustomValidation;
using System.ComponentModel.DataAnnotations;

namespace MoviesCRUDOperationMVC.Models.Entities
{
    public class Movie
    {
        public int Id { get; set; }

        [Required,MinLength(3),MaxLength(250)]
        public string Title { get; set; }

        public int Year { get; set; }
        public float? Rate { get; set; }

        [Required,MinLength(20),MaxLength(2500)]
        public string StoryLine { get; set; }

        [Required]
        public byte[] Poster { get; set; }

        public byte GenreId { get; set; }

        public Genre Genre { get; set; }
    }
}
