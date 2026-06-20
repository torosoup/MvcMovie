using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Models;

namespace MvcMovie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesApiController : ControllerBase
    {
        private readonly MvcMovieContext _context; // Add a reference to the MvcMovieContext to access the database.

        public MoviesApiController(MvcMovieContext context)
        {
            _context = context; // Inject the MvcMovieContext into the controller's constructor.
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            return await _context.Movie.ToListAsync(); // Return a list of movies from the database.
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            var movie = await _context.Movie.FindAsync(id); // Find a movie by its ID.

            if (movie == null)
            {
                return NotFound(); // Return 404 if the movie is not found.
            }
            return movie; // Return the movie if found.>

        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie(Movie movie)
        {
            _context.Movie.Add(movie); // Add the new movie to the database context.
            await _context.SaveChangesAsync(); // Save changes to the database.
            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie); // Return 201 Created with the location of the new movie.
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, Movie movie)
        {
            if (id != movie.Id)
            {
                return BadRequest(); // Return 400 Bad Request if the ID in the URL does not match the ID of the movie object.
            }
            _context.Entry(movie).State = EntityState.Modified; // Mark the movie as modified in the database context.
            try
            {
                await _context.SaveChangesAsync(); // Save changes to the database.
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Movie.AnyAsync(e => e.Id == id))
                {
                    return NotFound(); // Return 404 if the movie does not exist.
                }
                else
                {
                    throw; // Rethrow the exception if it's a concurrency issue.
                }
            }
            return NoContent(); // Return 204 No Content to indicate successful update.
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Movie.FindAsync(id); // Find the movie by its ID.
            if (movie == null)
            {
                return NotFound(); // Return 404 if the movie is not found.
            }
            _context.Movie.Remove(movie); // Remove the movie from the database context.
            await _context.SaveChangesAsync(); // Save changes to the database.
            return NoContent(); // Return 204 No Content to indicate successful deletion.
        }
    }
}
