using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MvcMovie.Controllers;
using MvcMovie.Models;
using Xunit;

namespace MvcMovie.Tests
{
    public class MoviesControllerTests
    {
        private MvcMovieContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<MvcMovieContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new MvcMovieContext(options);

            context.Movie.Add(new Movie { Title = "Test Movie", Genre = "Comedy", Price = 9.99M, Rating = "PG" });
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task Index_ReturnsViewWithMovies()
        {
            // Arrange
            var context = GetInMemoryContext();
            var controller = new MoviesController(context);

            // Act
            var result = await controller.Index(null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<MovieGenreViewModel>(viewResult.Model);
            Assert.Single(model.Movies!);
        }

        [Fact]
        public async Task Index_FiltersBySearchString()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MvcMovieContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new MvcMovieContext(options);
            context.Movie.AddRange(
                new Movie { Title = "Ghostbusters", Genre = "Comedy", Price = 9.99M, Rating = "PG" },
                new Movie { Title = "Rio Bravo", Genre = "Western", Price = 3.99M, Rating = "R" }
            );
            context.SaveChanges();

            var controller = new MoviesController(context);

            // Act
            var result = await controller.Index(null, "Ghost");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<MovieGenreViewModel>(viewResult.Model);
            Assert.Single(model.Movies!);
            Assert.Equal("Ghostbusters", model.Movies!.First().Title);
        }
    }
}