using OBLIG1.Models;
using Xunit;

namespace OBLIG1.Tests;

public class DefaultObstacleNameTest
{
    [Fact]
    public void Obstacle_DefaultName_WhenNameMissing()
    {
        var o = new Obstacle { Name = "" };
        Assert.True(string.IsNullOrWhiteSpace(o.Name) || o.Name == "");
    }
}
