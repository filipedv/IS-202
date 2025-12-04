using OBLIG1.Models;
using Xunit;

namespace OBLIG1.Tests;
//sjekker at hinder ikke trenger å ha et navn
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
}
