using Microsoft.EntityFrameworkCore;
using OBLIG1.Models; // ← gir tilgang til Obstacle-entity

namespace OBLIG1.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
    
    public DbSet<Obstacle> Obstacles => Set<Obstacle>(); // Tabellen "Obstacles"
}

