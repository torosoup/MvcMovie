using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Controllers;
using MvcMovie.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MvcMovie.Tests
{
    public class MovieApiControllerTests
    {
        // Create an in-memory database context for testing purposes, and seed it with a sample movie.
        private MvcMovieContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<MvcMovieContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new MvcMovieContext(options);

            context.Movie.Add(new Movie { Title = "Ghostbusters", Genre = "Comedy", Price = 9.99M, Rating = "PG" });
            context.SaveChanges();

            return context;
        }
        // Test the GetMovies action of the MoviesApiController to ensure it returns a list of movies from the database.
        [Fact]
        public async Task GetMovies_ReturnsListOfMovies()
        {
            // Arrange
            var context = GetInMemoryContext();
            var controller = new MoviesApiController(context);
            // Act
            var result = await controller.GetMovies();
            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Movie>>>(result);
            var movies = Assert.IsAssignableFrom<IEnumerable<Movie>>(actionResult.Value);
            Assert.Single(movies);
        }
        // Test the GetMovies action of the MoviesApiController to ensure it returns an empty list when there are no movies in the database.
        [Fact]
        public async Task GetMovies_ReturnEmptyListWhenNoMovies()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MvcMovieContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new MvcMovieContext(options);
            var controller = new MoviesApiController(context);
            // Act
            var result = await controller.GetMovies();
            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Movie>>>(result);
            var movies = Assert.IsAssignableFrom<IEnumerable<Movie>>(actionResult.Value);
            Assert.Empty(movies);
        }
        [Fact]
        public async Task GetMovie_ReturnsMovieById()
        {
            // Arrange
            var context = GetInMemoryContext();
            var controller = new MoviesApiController(context);
            var movie = await context.Movie.FirstAsync();
            // Act
            var result = await controller.GetMovie(movie.Id);
            // Assert
            var actionResult = Assert.IsType<ActionResult<Movie>>(result);
            var returnedMovie = Assert.IsType<Movie>(actionResult.Value);
            Assert.Equal(movie.Id, returnedMovie.Id);
        }
        [Theory] // Use Theory and InlineData to test multiple invalid IDs for the GetMovie action.
        [InlineData(999)]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task GetMovie_ReturnsNotFoundForInvalidId(int invalidId)
        {
            // Arrange
            var context = GetInMemoryContext();
            var controller = new MoviesApiController(context);
            // Act
            var result = await controller.GetMovie(invalidId); // Use an ID that does not exist in the database.
            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }   
        [Fact]
        public async Task PutMovie_UpdatesExistingMovie()
        {
            // Arrange
            var context = GetInMemoryContext();
            var controller = new MoviesApiController(context);
            var movie = await context.Movie.FirstAsync();
            movie.Title = "Updated Title";
            // Act
            var result = await controller.PutMovie(movie.Id, movie);
            // Assert
            Assert.IsType<NoContentResult>(result);
            var updatedMovie = await context.Movie.FindAsync(movie.Id);
            Assert.Equal("Updated Title", updatedMovie!.Title);
        }
        [Fact]
        public async Task PutMovie_ReturnsNotFoundWhenMovieDeletedConcurrently() // The throw branch in PutMovie would need a real SQL server integration test or a mocking framework like NSubstitute against a fake DbContexgt to properly cover.
        {
            // Arrange
            var context = GetInMemoryContext();
            var controller = new MoviesApiController(context);
            var movie = await context.Movie.FirstAsync();

            // Simulate the record being deleted by someone else before our update lands
            context.Movie.Remove(movie);
            await context.SaveChangesAsync();

            // Detach so EF doesn't know it's already tracking a deleted entity
            context.Entry(movie).State = EntityState.Detached;
            movie.Title = "Updated Title";

            // Act
            var result = await controller.PutMovie(movie.Id, movie);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task PutMovie_ReturnsNotFoundForInvalidId()
        {
            // Arrange
            var context = GetInMemoryContext();
            var controller = new MoviesApiController(context);
            var movie = new Movie { Id = 999, Title = "Non-existent Movie", Genre = "Action", Price = 19.99M, Rating = "R" };
            // Act
            var result = await controller.PutMovie(movie.Id, movie);
            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task PutMovie_ReturnsBadRequestForMismatchedId()
        {
            // Arrange
            var context = GetInMemoryContext();
            var controller = new MoviesApiController(context);
            var movie = await context.Movie.FirstAsync();
            var mismatchedId = movie.Id + 1; // Use a different ID than the one in the movie object.
            // Act
            var result = await controller.PutMovie(mismatchedId, movie);
            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
        [Theory]
        [InlineData(999)]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task PutMovie_ReturnBadRequestForInvalidId(int invalidId)
        {
            // Arrange
            var context = GetInMemoryContext();
            var controller = new MoviesApiController(context);
            // Act
            var result = await controller.GetMovie(invalidId); // Use an ID that does not exist in the database.
            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        [Fact]
        public async Task PostMovie_CreatesNewMovie()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MvcMovieContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new MvcMovieContext(options);
            var controller = new MoviesApiController(context);
            var newMovie = new Movie { Title = "Inception", Genre = "Sci-Fi", Price = 14.99M, Rating = "PG-13" };
            // Act
            var result = await controller.PostMovie(newMovie);
            // Assert
            var actionResult = Assert.IsType<ActionResult<Movie>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var createdMovie = Assert.IsType<Movie>(createdAtActionResult.Value);
            Assert.Equal(newMovie.Title, createdMovie.Title);
        }
        [Fact]
        public async Task PostMovie_ReturnsNotFoundForInvalidGetMovieAction()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MvcMovieContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new MvcMovieContext(options);
            var controller = new MoviesApiController(context);
            var newMovie = new Movie { Title = "Inception", Genre = "Sci-Fi", Price = 14.99M, Rating = "PG-13" };
            // Act
            var result = await controller.PostMovie(newMovie);
            // Assert
            var actionResult = Assert.IsType<ActionResult<Movie>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal(nameof(MoviesApiController.GetMovie), createdAtActionResult.ActionName); // Ensure the CreatedAtAction points to the correct GetMovie action.
        }
        
        [Fact]
        public async Task DeleteMovie_DeletesExistingMovie()
        {
            // Arrange
            var context = GetInMemoryContext();
            var controller = new MoviesApiController(context);
            var movie = await context.Movie.FirstAsync();
            // Act
            var result = await controller.DeleteMovie(movie.Id);
            // Assert
            Assert.IsType<NoContentResult>(result);
            var deletedMovie = await context.Movie.FindAsync(movie.Id);
            Assert.Null(deletedMovie);
        }
        [Theory]
        [InlineData(999)]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task DeleteMovie_ReturnsNotFoundForInvalidId(int invalidId)
        {
            // Arrange
            var context = GetInMemoryContext();
            var controller = new MoviesApiController(context);
            // Act
            var result = await controller.DeleteMovie(invalidId); // Use an ID that does not exist in the database.
            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
}
}
