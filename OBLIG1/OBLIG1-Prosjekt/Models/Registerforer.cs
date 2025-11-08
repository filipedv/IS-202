namespace OBLIG1.Models;

public class Registerforer
{
    public int Id { get; set; }
    public string Navn { get; set; } = "";
    public string Epost { get; set; }
    
    // Navigasjonsfelt
    public List<Obstacle> Obstacles { get; set; } = new();
}
    
