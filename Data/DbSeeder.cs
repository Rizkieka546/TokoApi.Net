using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TokoApi.Models;

namespace TokoApi.Data;

public static class DbSeeder
{
    public static async Task SeedAdminAsync(AppDbContext db, IConfiguration config)
    {
        await db.Database.MigrateAsync();

        if (await db.Users.AnyAsync())
            return;

        var hasher = new PasswordHasher<User>();

        var admin = new User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            Email = config["DefaultAdmin:Email"]!,
            Role = "admin",
            CreatedAt = DateTime.UtcNow
        };

        admin.PasswordHash = hasher.HashPassword(admin, config["DefaultAdmin:Password"]!);

        db.Users.Add(admin);
        await db.SaveChangesAsync();

        Console.WriteLine("=== Default Admin Created ===");
        Console.WriteLine($"Email: {admin.Email}");
        Console.WriteLine($"Password: {config["DefaultAdmin:Password"]}");
    }
}
