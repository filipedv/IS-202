using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using OBLIG1.Data;
using OBLIG1.Models;
using OBLIG1.Services;
using Xunit;

namespace OBLIG1.Tests;
//tester at alle approllene har riktige verdier
public class RolesShouldHaveCorrectValues
{
    [Fact]
    public void AppRoles_ShouldHaveCorrectValues()
    {
        // Assert
        Assert.Equal("Pilot", AppRoles.Pilot);
        Assert.Equal("Registerforer", AppRoles.Registrar);
        Assert.Equal("Admin", AppRoles.Admin);
    }

  
}
