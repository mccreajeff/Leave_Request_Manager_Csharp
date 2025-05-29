using LeaveRequestManager.Data;
using LeaveRequestManager.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveRequestManager.Services
{
    public class DatabaseInitializerService
    {
        public static async Task InitializeAsync()
        {
            using var context = new AppDbContext();
            
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();
            
            // Check if we already have users
            if (await context.Users.AnyAsync())
            {
                return; // Database has been seeded
            }
            
            // Create initial users
            var adminUser = new User
            {
                Username = "admin",
                EmployeeName = "Administrator",
                Email = "admin@company.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = "Admin",
                CreatedAt = DateTime.Now,
                IsActive = true
            };
            
            var employeeUser = new User
            {
                Username = "john.doe",
                EmployeeName = "John Doe",
                Email = "john.doe@company.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = "Employee",
                CreatedAt = DateTime.Now,
                IsActive = true
            };
            
            context.Users.Add(adminUser);
            context.Users.Add(employeeUser);
            
            await context.SaveChangesAsync();
            
            Console.WriteLine("Database initialized with default users:");
            Console.WriteLine("  Admin: username='admin', password='admin123'");
            Console.WriteLine("  Employee: username='john.doe', password='password123'");
        }
    }
} 