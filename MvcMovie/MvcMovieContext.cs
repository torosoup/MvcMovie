using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class MvcMovieContext(DbContextOptions<MvcMovieContext> options) : IdentityDbContext(options)
{
    public DbSet<MvcMovie.Models.Movie> Movie { get; set; } = default!;
}
