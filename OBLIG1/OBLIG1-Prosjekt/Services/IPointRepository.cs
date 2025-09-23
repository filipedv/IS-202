using OBLIG1.Models;

namespace OBLIG1.Services;

public interface IPointRepository
{
    IEnumerable<Point> All();
    void Add(Point p);
    Point? Get(Guid id);
}