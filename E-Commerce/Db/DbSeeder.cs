using E_Commerce.Models;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.Db;

public static class DbSeeder
{
    public static async Task SeedAdminASync(IServiceProvider serviceProvider)
    {
        // A scoped service belongs to a DI scope, typically one HTTP request. But seeding runs during application startup, outside an HTTP request.
        using var scope = serviceProvider.CreateScope();

        var provider = scope.ServiceProvider;

        var db = provider.GetRequiredService<AppDbContext>();
        var config = provider.GetRequiredService<IConfiguration>();
        var hasher = provider.GetRequiredService<IPasswordHasher<User>>();

        var adminEmail = config["Admin:Email"] ?? throw new InvalidOperationException("Admin:Email is not configured");

        var adminPassword = config["Admin:Password"]
                            ?? throw new InvalidOperationException(
                                "Admin:Password is not configured.");

        var name = config["Admin:Name"] ?? "System Admin";

        var normalizedEmail = adminEmail.Trim().ToLowerInvariant();


        var isAdminExist = db.Users.Any(u => u.Email == normalizedEmail);

        if (isAdminExist) return;

        var admin = new User
        {
            Name = name,
            Email = normalizedEmail,
            Role = Roles.Admin
        };
        admin.PasswordHash = hasher.HashPassword(admin, adminPassword);

        db.Users.Add(admin);

        await db.SaveChangesAsync();
    }
}