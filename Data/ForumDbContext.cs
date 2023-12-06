using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Skanaus.Auth.Model;
using Skanaus.Data.Entities;

namespace Skanaus.Data;

#nullable disable
public class ForumDbContext : IdentityDbContext<ForumRestUser>
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
        optionsBuilder.UseNpgsql(_configuration.GetValue<string>("PostgreSQLConnectionString"));
        //optionsBuilder.UseNpgsql(_configuration.GetConnectionString("PostgreSQL"));
    }
}