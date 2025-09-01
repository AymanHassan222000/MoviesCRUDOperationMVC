using Microsoft.EntityFrameworkCore;
using MoviesCRUDOperationMVC.Models.Entities;

namespace MoviesCRUDOperationMVC.Models.Data;

public class ApplictionDbContext : DbContext
{
    public ApplictionDbContext(DbContextOptions<ApplictionDbContext> options) : base(options) { }

    public DbSet<Genre> Genres { get; set; }
    public DbSet<Movie> Movies { get; set; }
}
