using E_Commerce.Features.Audit;
using E_Commerce.Features.Auth;
using E_Commerce.Features.Cart;
using E_Commerce.Features.Order;
using E_Commerce.Features.Products;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Db;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Cart> Cart => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // For Filteration and Sorting
        modelBuilder.Entity<Product>().HasIndex(p => p.Price);
        modelBuilder.Entity<Product>().HasIndex(p => p.Name);
        modelBuilder.Entity<Product>().HasIndex(p => p.Stock);

        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        // Audit log: append-only, keep indexes minimal (every index slows each write)
        modelBuilder.Entity<AuditLog>(b =>
        {
            b.Property(a => a.OldValues).HasColumnType("jsonb");
            b.Property(a => a.NewValues).HasColumnType("jsonb");
            b.HasIndex(a => new { a.EntityName, a.EntityId });
            b.HasIndex(a => a.Timestamp);
        });

        modelBuilder.Entity<CartItem>()
            .HasIndex(ci => new { ci.CartId, ci.ProductId })
            .IsUnique();

        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne() // no back navigation
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // For Computed Value
        modelBuilder.Entity<OrderItem>()
            .Property(i => i.TotalPrice)
            .HasComputedColumnSql("\"Quantity\" * \"UnitPrice\"", true)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderItem>().HasIndex(i => i.TotalPrice);

        modelBuilder.Entity<OrderItem>()
            .HasOne<Product>()
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

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