using Microsoft.EntityFrameworkCore;
using LeaveRequestManager.Models;

namespace LeaveRequestManager.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Update this connection string with your SQL Server details
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=LeaveRequestManagerDB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configure LeaveRequest entity
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.User)
                .WithMany()
                .HasForeignKey(lr => lr.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 