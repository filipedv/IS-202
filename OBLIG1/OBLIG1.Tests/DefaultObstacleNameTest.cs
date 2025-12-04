using OBLIG1.Models;
using Xunit;

namespace OBLIG1.Tests;

// Sjekker at hinder ikke trenger å ha et navn (default-verdi brukes)
public class DefaultObstacleNameTest
{
    [Fact]
    public void Obstacle_Name_ShouldAllowEmptyString()
    {
        // Arrange
        var obstacle = new Obstacle { Name = "" };

        // Act
        var nameIsEmpty = string.IsNullOrWhiteSpace(obstacle.Name);

        // Assert
        Assert.True(nameIsEmpty);
    }

    [Fact]
    public void Obstacle_Name_ShouldHaveDefaultEmptyString()
    {
        // Arrange & Act
        var obstacle = new Obstacle();

        // Assert - Default-verdien er satt til "" i modellen
        Assert.Equal("", obstacle.Name);
    }

    [Fact]
    public void ObstacleData_Name_ShouldAllowNull()
    {
        // Arrange
        var data = new ObstacleData { ObstacleName = null };

        // Assert
        Assert.Null(data.ObstacleName);
    }
}
