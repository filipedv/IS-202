using OBLIG1.Models;
using Xunit;

namespace OBLIG1.Tests;

// Tester at statusen på hinder er riktig
public class ObstacleStatusShouldHaveRightValue
{
// Tester at enum-verdiene for ObstacleStatus har riktig tallverdi (0,1,2)    
    [Fact]
    public void ObstacleStatus_ShouldHaveCorrectValues()
    {
        // Assert
        Assert.Equal(0, (int)ObstacleStatus.Pending);
        Assert.Equal(1, (int)ObstacleStatus.Approved);
        Assert.Equal(2, (int)ObstacleStatus.Rejected);
    }
    
// Tester at nye Obstacle-objekter får default status Pending
    [Fact]
    public void Obstacle_DefaultStatus_ShouldBePending()
    {
        // Arrange & Act
        var obstacle = new Obstacle { Name = "Test" };

        // Assert
        Assert.Equal(ObstacleStatus.Pending, obstacle.Status);
    }
    
// Tester at vi kan sette Status til alle gyldige enum-verdier
    [Theory]
    [InlineData(ObstacleStatus.Pending)]
    [InlineData(ObstacleStatus.Approved)]
    [InlineData(ObstacleStatus.Rejected)]
    public void Obstacle_Status_CanBeSetToAllValues(ObstacleStatus status)
    {
        // Arrange
        var obstacle = new Obstacle { Name = "Test" };

        // Act
        obstacle.Status = status;

        // Assert
        Assert.Equal(status, obstacle.Status);
    }
    
// Tester at ObstacleStatus-enumet bare har tre definerte verdier
    [Fact]
    public void ObstacleStatus_ShouldHaveThreeValues()
    {
        var values = Enum.GetValues<ObstacleStatus>();
        Assert.Equal(3, values.Length);
    }
}
