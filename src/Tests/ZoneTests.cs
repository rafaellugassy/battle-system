using BattleSystem.Models;
using Xunit;

namespace BattleSystem.Tests;

public class ZoneTests
{
    [Fact]
    public void ZONE_001_InitializesWithCorrectColorAndState()
    {
        // Arrange & Act
        var zone = new Zone("red", "attack");

        // Assert
        Assert.Equal("red", zone.Color);
        Assert.Equal("attack", zone.State);
    }

    [Fact]
    public void ZONE_002_InitializesWithDefendState()
    {
        // Arrange & Act
        var zone = new Zone("blue", "defend");

        // Assert
        Assert.Equal("blue", zone.Color);
        Assert.Equal("defend", zone.State);
    }

    [Fact]
    public void ZONE_003_InitializesWithEmptyState()
    {
        // Arrange & Act
        var zone = new Zone("green", "empty");

        // Assert
        Assert.Equal("green", zone.Color);
        Assert.Equal("empty", zone.State);
    }

    [Fact]
    public void ZONE_004_RejectsInvalidColor()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Zone("purple", "attack"));
    }

    [Fact]
    public void ZONE_005_RejectsInvalidState()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Zone("red", "explode"));
    }
}

// Made with Bob
