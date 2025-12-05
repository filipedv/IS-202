using System.ComponentModel.DataAnnotations;
using OBLIG1.Models;
using Xunit;

namespace OBLIG1.Tests;

// Sjekker at hinder ikke kan være høyere enn maks høyde
public class RejectValueAboveMax
{
    [Fact]
    public void ObstacleData_Height_ShouldRejectValueAboveMax()
    {
        // Arrange - Range(0, 200)
        var data = new ObstacleData { ObstacleHeight = 250 };
        var context = new ValidationContext(data);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(data, context, results, validateAllProperties: true);

        // Assert
        Assert.False(isValid, "Høyde over 200 i ObstacleData skal avvises");
        Assert.Contains(results, r => r.MemberNames.Contains("ObstacleHeight"));
    }

    [Fact]
    public void Obstacle_Height_ShouldRejectValueAboveMax()
    {
        // Arrange - Range(0, 200)
        var obstacle = new Obstacle { Name = "Test", Height = 201 };
        var context = new ValidationContext(obstacle);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(obstacle, context, results, validateAllProperties: true);

        // Assert
        Assert.False(isValid, "Høyde over 200 i Obstacle skal avvises");
        Assert.Contains(results, r => r.MemberNames.Contains("Height"));
    }

    [Fact]
    public void ObstacleData_Height_ShouldAcceptExactly200()
    {
        // Arrange
        var data = new ObstacleData { ObstacleHeight = 200 };
        var context = new ValidationContext(data);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(data, context, results, validateAllProperties: true);

        // Assert
        Assert.True(isValid, "Høyde på 200 skal aksepteres");
    }
}
