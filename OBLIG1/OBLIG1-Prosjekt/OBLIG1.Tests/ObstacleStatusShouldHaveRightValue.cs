using OBLIG1.Data;
using OBLIG1.Models;
using OBLIG1.Services;
using Xunit;
namespace OBLIG1.Tests;
//tester at statusen på hinder er rikitg
public class ObstacleStatusShouldHaveRightValue
{
    
    [Fact]
    public void ObstacleStatus_ShouldHaveCorrectValues()
    {
        // Assert
        Assert.Equal(0, (int)ObstacleStatus.Pending);
        Assert.Equal(1, (int)ObstacleStatus.Approved);
        Assert.Equal(2, (int)ObstacleStatus.Rejected);
    }
    
    [Fact]
    public void Obstacle_DefaultStatus_ShouldBePending()
    {
        // Arrange & Act
        var obstacle = new Obstacle { Name = "Test" };

        // Assert
        Assert.Equal(ObstacleStatus.Pending, obstacle.Status);
    }

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
}