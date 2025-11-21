using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Gir tilgang til IdentityDbContext (DbContext med innebygd Identity-støtte)
using Microsoft.EntityFrameworkCore; // Entity Framework Core – rammeverk for å jobbe mot databasen
using OBLIG1.Models; // Dine egne modellklasser (f.eks. ApplicationUser og Obstacle)


namespace OBLIG1.Data
{
    // ApplicationDbContext er databasen vår i C#-kode.
    // Den arver fra IdentityDbContext<ApplicationUser> slik at vi får med tabeller for brukere, roller osv. (ASP.NET Identity)
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // Konstruktør for ApplicationDbContext.
        // 'options' inneholder oppsett for databasen (connection string, database-type osv.)
        // De blir sendt videre til base-klassen (IdentityDbContext) via : base(options)
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            // Ingen ekstra kode her, all annet gjøres i base-klassen og via konfig i Program/Startup.
        }

        // DbSet for Obstacle-modellen.
        // Dette representerer en tabell i databasen der hver rad er et Obstacle-objekt.
        // Brukes blant annet til å hente, legge til, oppdatere og slette hinder.
        public DbSet<Obstacle> Obstacles { get; set; } = null!;
    }
}