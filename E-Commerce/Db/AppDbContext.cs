using E_Commerce.Features.Auth;
using E_Commerce.Features.Cart;
using E_Commerce.Features.Products;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Db;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Cart> Cart => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // For Filteration and Sorting
        modelBuilder.Entity<Product>().HasIndex(p => p.Price);
        modelBuilder.Entity<Product>().HasIndex(p => p.Name);
        modelBuilder.Entity<Product>().HasIndex(p => p.Stock);

        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        modelBuilder.Entity<CartItem>()
            .HasIndex(ci => new { ci.CartId, ci.ProductId })
            .IsUnique();

        // modelBuilder.Entity<Cart>().HasOne(c => c.User)
        //     .WithOne()
        //     .HasForeignKey<Cart>(c => c.UserId);
        //
        // modelBuilder.Entity<CartItem>().HasOne(ci => ci.Cart)
        //     .WithMany(c => c.Items)
        //     .HasForeignKey(ci => ci.CartId);
        //
        // modelBuilder.Entity<CartItem>().HasOne(ci => ci.Product)
        //     .WithMany()
        //     .HasForeignKey(ci => ci.ProductId);
    }
}