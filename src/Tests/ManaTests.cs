using BattleSystem.Models;
using Xunit;

namespace BattleSystem.Tests;

public class ManaTests
{
    [Fact]
    public void MANA_001_PlayerStartsWith10EmptyManaSlots()
    {
        // Arrange
        var card = CreateTestCard();
        var player = new Player("player1", card);

        // Assert
        Assert.Equal(10, player.ManaPool.Length);
        Assert.All(player.ManaPool, mana => Assert.True(mana.IsEmpty));
    }

    [Fact]
    public void MANA_002_AddingManaToEmptyPool()
    {
        // Arrange
        var card = CreateTestCard();
        var player = new Player("player1", card);

        // Act
        player.AddMana("red");

        // Assert
        var nonEmptyCount = player.ManaPool.Count(m => !m.IsEmpty);
        Assert.Equal(1, nonEmptyCount);
        Assert.Single(player.ManaPool, m => m.Color == "red");
    }

    [Fact]
    public void MANA_003_AddingManaWhenPoolIsFullRemovesOldest()
    {
        // Arrange
        var card = CreateTestCard();
        var player = new Player("player1", card);
        
        // Fill pool with specific pattern
        player.AddMana("red");    // 1 - oldest
        player.AddMana("blue");   // 2
        player.AddMana("green");  // 3
        player.AddMana("yellow"); // 4
        player.AddMana("red");    // 5
        player.AddMana("blue");   // 6
        player.AddMana("green");  // 7
        player.AddMana("yellow"); // 8
        player.AddMana("red");    // 9
        player.AddMana("blue");   // 10 - newest

        // Act - add one more, should remove oldest red
        player.AddMana("yellow");

        // Assert
        var redCount = player.ManaPool.Count(m => m.Color == "red");
        var yellowCount = player.ManaPool.Count(m => m.Color == "yellow");
        Assert.Equal(2, redCount); // Lost the oldest red
        Assert.Equal(3, yellowCount); // Gained one yellow
    }

    [Fact]
    public void MANA_004_ColorlessCardGeneratesNoMana()
    {
        // Arrange
        var card = CreateColorlessCard();
        var player = new Player("player1", card);
        var initialEmptyCount = player.ManaPool.Count(m => m.IsEmpty);

        // Act
        player.AddMana(card.Color);

        // Assert
        var finalEmptyCount = player.ManaPool.Count(m => m.IsEmpty);
        Assert.Equal(initialEmptyCount, finalEmptyCount);
    }

    [Fact]
    public void MANA_005_SpendingColoredManaConsumesOldestOfThatColor()
    {
        // Arrange
        var card = CreateTestCard();
        var player = new Player("player1", card);
        
        player.AddMana("red");   // 1 - oldest red
        player.AddMana("blue");  // 2
        player.AddMana("red");   // 3 - middle red
        player.AddMana("red");   // 4 - newest red

        var manaCost = new ManaCost("red", 2);

        // Act
        var success = player.TrySpendMana(manaCost);

        // Assert
        Assert.True(success);
        var redCount = player.ManaPool.Count(m => m.Color == "red");
        Assert.Equal(1, redCount); // Should have 1 red left (the newest)
        Assert.Single(player.ManaPool, m => m.Color == "blue");
    }

    [Fact]
    public void MANA_006_SpendingAnyManaConsumesOldestOverall()
    {
        // Arrange
        var card = CreateTestCard();
        var player = new Player("player1", card);
        
        player.AddMana("red");    // 1 - oldest
        player.AddMana("blue");   // 2
        player.AddMana("green");  // 3

        var manaCost = new ManaCost("any", 1);

        // Act
        var success = player.TrySpendMana(manaCost);

        // Assert
        Assert.True(success);
        Assert.Empty(player.ManaPool.Where(m => m.Color == "red")); // Red was oldest, should be gone
        Assert.Single(player.ManaPool, m => m.Color == "blue");
        Assert.Single(player.ManaPool, m => m.Color == "green");
    }

    [Fact]
    public void MANA_007_SpendingMoreManaThanAvailableFails()
    {
        // Arrange
        var card = CreateTestCard();
        var player = new Player("player1", card);
        
        player.AddMana("red");
        player.AddMana("red");

        var manaCost = new ManaCost("red", 3);

        // Act
        var success = player.TrySpendMana(manaCost);

        // Assert
        Assert.False(success);
        var redCount = player.ManaPool.Count(m => m.Color == "red");
        Assert.Equal(2, redCount); // Mana should be unchanged
    }

    [Fact]
    public void MANA_008_ManaRequirementCheckPassesWhenManaIsPresent()
    {
        // Arrange
        var card = CreateTestCard();
        var player = new Player("player1", card);
        
        player.AddMana("red");
        player.AddMana("red");
        player.AddMana("blue");

        var requirement = new ManaRequirement("red", 2);

        // Act
        var canMeet = player.CanMeetRequirement(requirement);

        // Assert
        Assert.True(canMeet);
        // Mana should not be consumed
        Assert.Equal(2, player.ManaPool.Count(m => m.Color == "red"));
    }

    [Fact]
    public void MANA_009_ManaRequirementCheckFailsWhenManaIsInsufficient()
    {
        // Arrange
        var card = CreateTestCard();
        var player = new Player("player1", card);
        
        player.AddMana("red");
        player.AddMana("blue");

        var requirement = new ManaRequirement("red", 2);

        // Act
        var canMeet = player.CanMeetRequirement(requirement);

        // Assert
        Assert.False(canMeet);
    }

    [Fact]
    public void MANA_010_ManaRequirementCheckForAnyPassesWithMixedColors()
    {
        // Arrange
        var card = CreateTestCard();
        var player = new Player("player1", card);
        
        player.AddMana("red");
        player.AddMana("blue");
        player.AddMana("green");

        var requirement = new ManaRequirement("any", 3);

        // Act
        var canMeet = player.CanMeetRequirement(requirement);

        // Assert
        Assert.True(canMeet);
        // Mana should not be consumed
        Assert.Equal(3, player.ManaPool.Count(m => !m.IsEmpty));
    }

    [Fact]
    public void MANA_011_ManaPersistsBetweenBattles()
    {
        // Arrange
        var card = CreateTestCard("red");
        var player = new Player("player1", card);
        
        // Simulate end of battle 1
        player.AddMana("red");
        player.AddMana("blue");

        var initialRedCount = player.ManaPool.Count(m => m.Color == "red");
        var initialBlueCount = player.ManaPool.Count(m => m.Color == "blue");

        // Act - simulate end of battle 2
        player.AddMana(card.Color); // Add red from card

        // Assert
        var finalRedCount = player.ManaPool.Count(m => m.Color == "red");
        var finalBlueCount = player.ManaPool.Count(m => m.Color == "blue");
        
        Assert.Equal(initialRedCount + 1, finalRedCount);
        Assert.Equal(initialBlueCount, finalBlueCount);
    }

    // Helper methods
    private Card CreateTestCard(string color = "red")
    {
        var zones = new[]
        {
            new Zone("red", "attack"),
            new Zone("blue", "empty"),
            new Zone("yellow", "empty"),
            new Zone("green", "empty")
        };

        var power = new Power(
            "test-power",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.ADDITIONAL_DAMAGE, EffectTarget.OPPONENT, 1)
        );

        return new Card("test-card", "Test Card", color, 5, zones, new List<Power> { power });
    }

    private Card CreateColorlessCard()
    {
        return CreateTestCard("colorless");
    }
}

// Made with Bob
