using FootballStats.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace FootballStats.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        
    }
    
    public DbSet<Match> Matches { get; set; }
    public DbSet<User> Users { get; set; }
}