using System.ComponentModel.DataAnnotations;
using OBLIG1.Models;
using Xunit;

namespace OBLIG1.Tests;

// Sjekker at høyden på hinder ikke kan være et negativt tall
public class RejectNegativeHeight
{
    [Fact]
    public void ObstacleData_Height_ShouldRejectNegativeValue()
    {
        // Arrange
        var data = new ObstacleData { ObstacleHeight = -10 };
        var context = new ValidationContext(data);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(data, context, results, validateAllProperties: true);

        // Assert
        Assert.False(isValid, "Negativ høyde i ObstacleData skal avvises");
        Assert.Contains(results, r => r.MemberNames.Contains("ObstacleHeight"));
    }

    [Fact]
    public void Obstacle_Height_ShouldRejectNegativeValue()
    {
        // Arrange
        var obstacle = new Obstacle { Name = "Test", Height = -5 };
        var context = new ValidationContext(obstacle);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(obstacle, context, results, validateAllProperties: true);

        // Assert
        Assert.False(isValid, "Negativ høyde i Obstacle skal avvises");
        Assert.Contains(results, r => r.MemberNames.Contains("Height"));
    }
}
