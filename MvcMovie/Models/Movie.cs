using System.ComponentModel.DataAnnotations;

namespace MvcMovie.Models
{
    public class Movie // Defines the properties of stored data in the database.
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
        public string? Genre { get; set; }
        public decimal Price { get; set; }
    }
}
