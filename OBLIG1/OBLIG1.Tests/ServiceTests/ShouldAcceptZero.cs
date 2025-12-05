using System.ComponentModel.DataAnnotations;
using OBLIG1.Models;
using Xunit;

namespace OBLIG1.Tests;

// Tester at domenemodellen Obstacle godtar høyde 0
public class ShouldAcceptZero
{
    [Fact]
    public void Obstacle_Height_ShouldAcceptZero()
    {
        // Arrange
        var obstacle = new Obstacle { Name = "Test", Height = 0 };
        var context = new ValidationContext(obstacle);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(obstacle, context, results, validateAllProperties: true);

        // Assert
        Assert.True(isValid);
    }
    
// Tester at ObstacleData godtar høyde 0
    [Fact]
    public void ObstacleData_Height_ShouldAcceptZero()
    {
        // Arrange
        var data = new ObstacleData { ObstacleHeight = 0 };
        var context = new ValidationContext(data);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(data, context, results, validateAllProperties: true);

        // Assert
        Assert.True(isValid);
    }
    
// Tester at ObstacleData godtar null som høyde (feltet er valgfritt)
    [Fact]
    public void ObstacleData_Height_ShouldAcceptNull()
    {
        // Arrange
        var data = new ObstacleData { ObstacleHeight = null };
        var context = new ValidationContext(data);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(data, context, results, validateAllProperties: true);

        // Assert
        Assert.True(isValid, "Null-høyde skal aksepteres (valgfritt felt)");
    }
}
