using MvcMovie.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MvcMovie.Tests
{
    public class MovieTests
    {
        [Theory]
        [InlineData(null, "Comedy", 9.99, "PG")]                    // Title missing (Required)
        [InlineData("AB", "Comedy", 9.99, "PG")]                    // Title too short (StringLength min)
        [InlineData("Valid Title", "comedy", 9.99, "PG")]           // Genre lowercase (RegularExpression)
        [InlineData("Valid Title", null, 9.99, "PG")]                // Genre missing (Required)
        [InlineData("Valid Title", "Comedy", 0, "PG")]               // Price below Range(1,100)
        [InlineData("Valid Title", "Comedy", 150, "PG")]             // Price above Range(1,100)
        [InlineData("Valid Title", "Comedy", 9.99, "pg")]            // Rating lowercase (RegularExpression)
        [InlineData("Valid Title", "Comedy", 9.99, null)]            // Rating missing (Required)
        public void Movie_FailsValidationForInvalidData(string? title, string? genre, decimal price, string? rating)
        {
            // Arrange
            var movie = new Movie { Title = title, Genre = genre, Price = price, Rating = rating };
            var context = new ValidationContext(movie);
            var results = new List<ValidationResult>();

            // Act
            bool isValid = Validator.TryValidateObject(movie, context, results, true);

            // Assert
            Assert.False(isValid);
        }
    }
}
