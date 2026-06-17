using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MvcMovie.Data;

public class MvcMovieContext(DbContextOptions<MvcMovieContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<MvcMovie.Models.Movie> Movie { get; set; } = default!;
}
