using Microsoft.EntityFrameworkCore;
using Skanaus.Data.Entities;

namespace Skanaus.Data;

public class ForumDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    public DbSet<Kitchen> Kitchens { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Comment> Comments { get; set; }

    public ForumDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("PostgreSQL"));
    }
}