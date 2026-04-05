using Xunit;
using BattleSystem.Models;

namespace BattleSystem.Tests;

public class GreatestHitTests
{
    [Fact]
    public void GHIT_001_SingleAttackVsEmptyHitEmptyResult()
    {
        // Arrange
        var attackerZones = new[]
        {
            new Zone("red", "attack"),
            new Zone("blue", "empty"),
            new Zone("green", "empty"),
            new Zone("yellow", "empty")
        };
        
        var defenderZones = new[]
        {
            new Zone("red", "empty"),
            new Zone("blue", "empty"),
            new Zone("green", "empty"),
            new Zone("yellow", "empty")
        };
        
        var attackerCard = new Card("c1", "Attacker", "red", 5, attackerZones, new List<Power> { CreateDummyPower() });
        var defenderCard = new Card("c2", "Defender", "blue", 5, defenderZones, new List<Power> { CreateDummyPower() });
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert - HIT_EMPTY deals full damage (5)
        Assert.Equal(15, defender.Hp); // 20 - 5 = 15
    }

    [Fact]
    public void GHIT_002_SingleAttackVsAttackHitAttackResult()
    {
        // Arrange
        var attackerZones = new[]
        {
            new Zone("red", "attack"),
            new Zone("blue", "empty"),
            new Zone("green", "empty"),
            new Zone("yellow", "empty")
        };
        
        var defenderZones = new[]
        {
            new Zone("red", "attack"),
            new Zone("blue", "empty"),
            new Zone("green", "empty"),
            new Zone("yellow", "empty")
        };
        
        var attackerCard = new Card("c1", "Attacker", "red", 5, attackerZones, new List<Power> { CreateDummyPower() });
        var defenderCard = new Card("c2", "Defender", "blue", 5, defenderZones, new List<Power> { CreateDummyPower() });
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert - HIT_ATTACK deals half damage (floor of 5/2 = 2)
        Assert.Equal(18, defender.Hp); // 20 - 2 = 18
    }

    [Fact]
    public void GHIT_003_SingleAttackVsDefendNoHitResult()
    {
        // Arrange
        var attackerZones = new[]
        {
            new Zone("red", "attack"),
            new Zone("blue", "empty"),
            new Zone("green", "empty"),
            new Zone("yellow", "empty")
        };
        
        var defenderZones = new[]
        {
            new Zone("red", "defend"),
            new Zone("blue", "empty"),
            new Zone("green", "empty"),
            new Zone("yellow", "empty")
        };
        
        var attackerCard = new Card("c1", "Attacker", "red", 5, attackerZones, new List<Power> { CreateDummyPower() });
        var defenderCard = new Card("c2", "Defender", "blue", 5, defenderZones, new List<Power> { CreateDummyPower() });
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert - NO_HIT deals no damage
        Assert.Equal(20, defender.Hp); // No damage
    }

    [Fact]
    public void GHIT_004_MultipleAttacksOneEmptyOneAttackBestResultWins()
    {
        // Arrange
        var attackerZones = new[]
        {
            new Zone("red", "attack"),
            new Zone("blue", "attack"),
            new Zone("green", "empty"),
            new Zone("yellow", "empty")
        };
        
        var defenderZones = new[]
        {
            new Zone("red", "empty"),
            new Zone("blue", "attack"),
            new Zone("green", "empty"),
            new Zone("yellow", "empty")
        };
        
        var attackerCard = new Card("c1", "Attacker", "red", 5, attackerZones, new List<Power> { CreateDummyPower() });
        var defenderCard = new Card("c2", "Defender", "blue", 5, defenderZones, new List<Power> { CreateDummyPower() });
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert - HIT_EMPTY (best result) deals full damage (5)
        Assert.Equal(15, defender.Hp); // 20 - 5 = 15
    }

    [Fact]
    public void GHIT_005_MultipleAttacksOneBlockedOneAttackVsAttack()
    {
        // Arrange
        var attackerZones = new[]
        {
            new Zone("red", "attack"),
            new Zone("blue", "attack"),
            new Zone("green", "empty"),
            new Zone("yellow", "empty")
        };
        
        var defenderZones = new[]
        {
            new Zone("red", "defend"),
            new Zone("blue", "attack"),
            new Zone("green", "empty"),
            new Zone("yellow", "empty")
        };
        
        var attackerCard = new Card("c1", "Attacker", "red", 5, attackerZones, new List<Power> { CreateDummyPower() });
        var defenderCard = new Card("c2", "Defender", "blue", 5, defenderZones, new List<Power> { CreateDummyPower() });
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert - HIT_ATTACK (best available) deals half damage (floor of 5/2 = 2)
        Assert.Equal(18, defender.Hp); // 20 - 2 = 18
    }

    [Fact]
    public void GHIT_006_MultipleAttacksAllBlockedNoHit()
    {
        // Arrange
        var attackerZones = new[]
        {
            new Zone("red", "attack"),
            new Zone("blue", "attack"),
            new Zone("green", "attack"),
            new Zone("yellow", "attack")
        };
        
        var defenderZones = new[]
        {
            new Zone("red", "defend"),
            new Zone("blue", "defend"),
            new Zone("green", "defend"),
            new Zone("yellow", "defend")
        };
        
        var attackerCard = new Card("c1", "Attacker", "red", 5, attackerZones, new List<Power> { CreateDummyPower() });
        var defenderCard = new Card("c2", "Defender", "blue", 5, defenderZones, new List<Power> { CreateDummyPower() });
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert - NO_HIT deals no damage
        Assert.Equal(20, defender.Hp); // No damage
    }

    [Fact]
    public void GHIT_007_MultipleEmptyHitsCountAsOneHitEmpty()
    {
        // Arrange
        var attackerZones = new[]
        {
            new Zone("red", "attack"),
            new Zone("blue", "attack"),
            new Zone("green", "empty"),
            new Zone("yellow", "empty")
        };
        
        var defenderZones = new[]
        {
            new Zone("red", "empty"),
            new Zone("blue", "empty"),
            new Zone("green", "empty"),
            new Zone("yellow", "empty")
        };
        
        var attackerCard = new Card("c1", "Attacker", "red", 5, attackerZones, new List<Power> { CreateDummyPower() });
        var defenderCard = new Card("c2", "Defender", "blue", 5, defenderZones, new List<Power> { CreateDummyPower() });
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert - Multiple empty hits still count as one HIT_EMPTY, dealing damage once (5)
        Assert.Equal(15, defender.Hp); // 20 - 5 = 15 (not 20 - 10)
    }

    [Fact]
    public void GHIT_008_NoAttackZonesNoHit()
    {
        // Arrange
        var attackerZones = new[]
        {
            new Zone("red", "defend"),
            new Zone("blue", "defend"),
            new Zone("green", "empty"),
            new Zone("yellow", "empty")
        };
        
        var defenderZones = new[]
        {
            new Zone("red", "empty"),
            new Zone("blue", "empty"),
            new Zone("green", "empty"),
            new Zone("yellow", "empty")
        };
        
        var attackerCard = new Card("c1", "Attacker", "red", 5, attackerZones, new List<Power> { CreateDummyPower() });
        var defenderCard = new Card("c2", "Defender", "blue", 5, defenderZones, new List<Power> { CreateDummyPower() });
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert - NO_HIT deals no damage
        Assert.Equal(20, defender.Hp); // No damage
    }

    // Helper method
    private Power CreateDummyPower()
    {
        return new Power(
            "dummy",
            new Condition(ConditionType.AFTER_TURN_RESOLVES),
            new Effect(EffectType.HEAL, EffectTarget.SELF, 0, null),
            null,
            null
        );
    }
}

// Made with Bob
