using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using OBLIG1.Data;
using OBLIG1.Models;
using OBLIG1.Services;
using Xunit;

namespace OBLIG1.Tests;
//sjekker at hinder ikke kan være høyere enn maks høyde
public class RejectValueAboveMax
{
    [Fact]
    public void ObstacleData_Height_ShouldRejectValueAboveMax() 
    {
        // Arrange
        var data = new ObstacleData { ObstacleHeight = 250 };
        var context = new ValidationContext(data);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(data, context, results, validateAllProperties: true);

        // Assert
        Assert.False(isValid,"Høyde over 200 i ObstacleData skal avvises");
    }
}







