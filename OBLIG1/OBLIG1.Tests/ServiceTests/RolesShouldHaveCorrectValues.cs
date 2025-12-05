using OBLIG1.Models;
using Xunit;

namespace OBLIG1.Tests;

// Tester at alle approllene har riktige verdier
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
    
// Tester at det finnes akkurat tre definerte roller i AppRoles
    [Fact]
    public void AppRoles_ShouldHaveThreeRoles()
    {
        var roleFields = typeof(AppRoles)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.IsLiteral && !f.IsInitOnly)
            .ToList();

        Assert.Equal(3, roleFields.Count);
    }
}
