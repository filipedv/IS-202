using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using OBLIG1.Data;
using OBLIG1.Models;
using OBLIG1.Services;
using Xunit;

namespace OBLIG1.Tests;

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
   
}