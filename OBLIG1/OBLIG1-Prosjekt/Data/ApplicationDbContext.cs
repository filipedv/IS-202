using Microsoft.EntityFrameworkCore;
using OBLIG1.Models;

namespace OBLIG1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Existing table
        public DbSet<Obstacle> Obstacles => Set<Obstacle>();

        // Table for users
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed admin and default users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin1",
                    Email = "admin@example.com",
                    Role = "Admin",
                    IsBlocked = false,
                    Password = "admin123"
                },
                new User
                {
                    Id = 2,
                    Username = "register1",
                    Email = "register@example.com",
                    Role = "Registerer",
                    IsBlocked = false,
                    Password = "register123"
                },
                new User
                {
                    Id = 3,
                    Username = "pilot",
                    Email = "pilot@example.com",
                    Role = "Pilot",
                    IsBlocked = false,
                    Password = "pilot123"
                }
            );
        }
    }
}