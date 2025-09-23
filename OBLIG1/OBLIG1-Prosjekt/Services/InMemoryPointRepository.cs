using OBLIG1.Models;

namespace OBLIG1.Services;

public class InMemoryPointRepository : IPointRepository
{
    private readonly List<Point> _items = new();
    private readonly object _lock = new();

    public IEnumerable<Point> All()
    {
        lock (_lock) return _items.ToList();
    }

    public void Add(Point p)
    {
        lock (_lock) _items.Add(p);
    }

    public Point? Get(Guid id)
    {
        lock (_lock) return _items.FirstOrDefault(x => x.Id == id);
    }
}