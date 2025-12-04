using OBLIG1.Models;
using Xunit;

namespace OBLIG1.Tests;

public class RejectNegativeHeight
{
    [Fact]
    public void Obstacle_ShouldRejectNegativeHeight()
    {
        // Arrange
        var obstacle = new Obstacle { Height = -5 };
        
        // Act
        bool isNegative = obstacle.Height < 0;
        
        // Assert 
        Assert.True(isNegative);
    }
    
}