using Microsoft.EntityFrameworkCore;
using MikesPaging.WebApi.Models;

namespace MikesPaging.WebApi.Data;

public class WebApiExampleDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    public DbSet<Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Seed();
    }
}