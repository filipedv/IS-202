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

    [Fact]
    public void AppRoles_ShouldHaveThreeRoles()
    {
        // Sjekk at vi har definert akkurat 3 roller
        var roleFields = typeof(AppRoles)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.IsLiteral && !f.IsInitOnly)
            .ToList();

        Assert.Equal(3, roleFields.Count);
    }
}
