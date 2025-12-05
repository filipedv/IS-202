using System.ComponentModel.DataAnnotations;
using OBLIG1.Models;
using Xunit;

namespace OBLIG1.Tests;

// Tester at navnet på hinder ikke kan være over 100 tegn
public class RejectTooLongObstacleNames
{
    [Fact]
    public void ObstacleData_Name_ShouldRejectTooLongString()
    {
        // Arrange - MaxLength(100)
        var data = new ObstacleData { ObstacleName = new string('A', 101) };
        var context = new ValidationContext(data);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(data, context, results, validateAllProperties: true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("ObstacleName"));
    }

    [Fact]
    public void Obstacle_Name_ShouldRejectTooLongString()
    {
        // Arrange - StringLength(100)
        var obstacle = new Obstacle { Name = new string('A', 101) };
        var context = new ValidationContext(obstacle);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(obstacle, context, results, validateAllProperties: true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("Name"));
    }

    [Fact]
    public void ObstacleData_Name_ShouldAcceptExactly100Characters()
    {
        // Arrange
        var data = new ObstacleData { ObstacleName = new string('A', 100) };
        var context = new ValidationContext(data);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(data, context, results, validateAllProperties: true);

        // Assert
        Assert.True(isValid);
    }
}
