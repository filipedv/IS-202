using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using OBLIG1.Data;
using OBLIG1.Models;
using OBLIG1.Services;
using Xunit;

namespace OBLIG1.Tests;
//tester at navnet på hinder ikke kan være over 100
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
}