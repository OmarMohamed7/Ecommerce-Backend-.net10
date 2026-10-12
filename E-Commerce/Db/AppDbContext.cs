using E_Commerce.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Db;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
}