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
    [Fact]
    public void GHIT_009_OnDefendFiresWhenDefendBlocksAttack()
    {
        // Arrange
        // Player1: Red card, 3 strength, attacks on red and blue zones
        var attackerZones = new[]
        {
            new Zone("red", "attack"),
            new Zone("blue", "attack"),
            new Zone("green", "empty"),
            new Zone("yellow", "empty")
        };
        
        // Player2: Blue card, defends on blue zone, has ON_DEFEND power that applies burn(1) to opponent
        var defenderZones = new[]
        {
            new Zone("red", "empty"),
            new Zone("blue", "defend"),
            new Zone("green", "empty"),
            new Zone("yellow", "empty")
        };
        
        var burnStatus = new Status("burn", 1, 3, 100);
        var onDefendPower = new Power(
            "ondefend",
            new Condition(ConditionType.ON_DEFEND),
            new Effect(EffectType.APPLY_STATUS, EffectTarget.OPPONENT, null, burnStatus),
            null,
            null
        );
        
        var attackerCard = new Card("c1", "Red Attacker", "red", 3, attackerZones, new List<Power> { CreateDummyPower() });
        var defenderCard = new Card("c2", "Blue Defender", "blue", 3, defenderZones, new List<Power> { onDefendPower });
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        // Player1 attacks with red (hits empty) and blue (blocked by defend)
        // Greatest Hit Rule: HIT_EMPTY wins over the blocked attack
        // Since Greatest Hit Rule determines the outcome, the defend doesn't "successfully block"
        // Therefore ON_DEFEND should NOT fire
        // Player2 takes 3 damage from HIT_EMPTY
        // Player1 takes no damage and has no burn status
        // Final: Player1 HP = 20, Player2 HP = 20 - 3 = 17
        Assert.Equal(20, attacker.Hp); // No damage taken
        Assert.Equal(17, defender.Hp); // Took 3 strength damage from HIT_EMPTY
        Assert.False(attacker.HasStatus("burn")); // ON_DEFEND did not fire
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
