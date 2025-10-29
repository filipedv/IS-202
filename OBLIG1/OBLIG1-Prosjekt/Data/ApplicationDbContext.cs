using Microsoft.EntityFrameworkCore;

namespace OBLIG1.Data; // ← Bytt til ditt faktiske namespace om nødvendig

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

    // Minst én DbSet så migrasjonen ikke blir tom:
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}

// Eksempel-entity
public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public bool Done { get; set; }
}