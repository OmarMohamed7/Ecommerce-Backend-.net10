using Microsoft.EntityFrameworkCore;
using E_Commerce.Features.Auth;
using E_Commerce.Features.Products;

namespace E_Commerce.Db;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // For Filteration and Sorting
        modelBuilder.Entity<Product>().HasIndex(p => p.Price);
        modelBuilder.Entity<Product>().HasIndex(p => p.Name);
        modelBuilder.Entity<Product>().HasIndex(p => p.Stock);

        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
    }
}