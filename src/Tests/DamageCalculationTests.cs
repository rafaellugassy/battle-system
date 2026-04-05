using Xunit;
using BattleSystem.Models;

namespace BattleSystem.Tests;

public class DamageCalculationTests
{
    [Fact]
    public void DMG_001_FullStrengthDamageOnHitEmpty()
    {
        // Arrange
        var attackerCard = CreateCard("c1", "red", 5, "attack", "empty", "empty", "empty");
        var defenderCard = CreateCard("c2", "blue", 5, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(15, defender.Hp); // 20 - 5 = 15
    }

    [Fact]
    public void DMG_002_HalfStrengthDamageOnHitAttack()
    {
        // Arrange
        var attackerCard = CreateCard("c1", "red", 5, "attack", "empty", "empty", "empty");
        var defenderCard = CreateCard("c2", "blue", 5, "attack", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(18, defender.Hp); // 20 - floor(5/2) = 20 - 2 = 18
    }

    [Fact]
    public void DMG_003_WeakenHalvesStrengthDamage()
    {
        // Arrange
        var attackerCard = CreateCard("c1", "red", 6, "attack", "empty", "empty", "empty");
        var defenderCard = CreateCard("c2", "blue", 6, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        attacker.AddStatus(new Status("weaken", 1, null, 100));
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(17, defender.Hp); // 20 - floor(6 * 0.5) = 20 - 3 = 17
    }

    [Fact]
    public void DMG_004_WeakenHalvesAbilityDamage()
    {
        // Arrange
        var abilityPower = new Power(
            "ability",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.ABILITY_DAMAGE, EffectTarget.OPPONENT, 6, null),
            null,
            null
        );
        
        var attackerCard = CreateCardWithPower("c1", "red", 10, "attack", "empty", "empty", "empty", abilityPower);
        var defenderCard = CreateCard("c2", "blue", 5, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        attacker.AddStatus(new Status("weaken", 1, null, 100));
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(17, defender.Hp); // 20 - floor(6 * 0.5) = 20 - 3 = 17
    }

    [Fact]
    public void DMG_005_WeakenDoesNotApplyToAdditionalDamage()
    {
        // Arrange
        var additionalDamagePower = new Power(
            "additional",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.ADDITIONAL_DAMAGE, EffectTarget.OPPONENT, 4, null),
            null,
            null
        );
        
        var attackerCard = CreateCardWithPower("c1", "red", 2, "attack", "empty", "empty", "empty", additionalDamagePower);
        var defenderCard = CreateCard("c2", "blue", 5, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        attacker.AddStatus(new Status("weaken", 1, null, 100));
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        // Strength damage: 2 * 0.5 (weaken) = 1
        // Additional damage: 4 (weaken doesn't apply)
        // Total: 1 + 4 = 5
        Assert.Equal(15, defender.Hp); // 20 - 5 = 15
    }

    [Fact]
    public void DMG_006_ProtectHalvesStrengthDamage()
    {
        // Arrange
        var attackerCard = CreateCard("c1", "red", 6, "attack", "empty", "empty", "empty");
        var defenderCard = CreateCard("c2", "blue", 6, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        defender.AddStatus(new Status("protect", 1, null, 100));
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(17, defender.Hp); // 20 - floor(6 * 0.5) = 20 - 3 = 17
    }

    [Fact]
    public void DMG_007_ProtectHalvesAdditionalDamage()
    {
        // Arrange
        var additionalDamagePower = new Power(
            "additional",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.ADDITIONAL_DAMAGE, EffectTarget.OPPONENT, 4, null),
            null,
            null
        );
        
        var attackerCard = CreateCardWithPower("c1", "red", 0, "attack", "empty", "empty", "empty", additionalDamagePower);
        var defenderCard = CreateCard("c2", "blue", 5, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        defender.AddStatus(new Status("protect", 1, null, 100));
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(18, defender.Hp); // 20 - floor(4 * 0.5) = 20 - 2 = 18
    }

    [Fact]
    public void DMG_008_VulnerableDoublesStrengthDamage()
    {
        // Arrange
        var attackerCard = CreateCard("c1", "red", 4, "attack", "empty", "empty", "empty");
        var defenderCard = CreateCard("c2", "blue", 4, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        defender.AddStatus(new Status("vulnerable", 1, null, 100));
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(12, defender.Hp); // 20 - (4 * 2) = 12
    }

    [Fact]
    public void DMG_009_VulnerableDoublesAdditionalDamage()
    {
        // Arrange
        var additionalDamagePower = new Power(
            "additional",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.ADDITIONAL_DAMAGE, EffectTarget.OPPONENT, 3, null),
            null,
            null
        );
        
        var attackerCard = CreateCardWithPower("c1", "red", 0, "attack", "empty", "empty", "empty", additionalDamagePower);
        var defenderCard = CreateCard("c2", "blue", 5, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        defender.AddStatus(new Status("vulnerable", 1, null, 100));
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(14, defender.Hp); // 20 - (3 * 2) = 14
    }

    [Fact]
    public void DMG_010_WeakenAndVulnerableTogetherOnStrengthDamage()
    {
        // Arrange
        var attackerCard = CreateCard("c1", "red", 4, "attack", "empty", "empty", "empty");
        var defenderCard = CreateCard("c2", "blue", 4, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        attacker.AddStatus(new Status("weaken", 1, null, 100));
        defender.AddStatus(new Status("vulnerable", 1, null, 101));
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(16, defender.Hp); // 20 - floor(4 * 0.5 * 2) = 20 - 4 = 16
    }

    [Fact]
    public void DMG_011_ProtectAndVulnerableTogetherCancelOut()
    {
        // Arrange
        var attackerCard = CreateCard("c1", "red", 4, "attack", "empty", "empty", "empty");
        var defenderCard = CreateCard("c2", "blue", 4, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        defender.AddStatus(new Status("protect", 1, null, 100));
        defender.AddStatus(new Status("vulnerable", 1, null, 101));
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(16, defender.Hp); // 20 - floor(4 * 0.5 * 2) = 20 - 4 = 16
    }

    [Fact]
    public void DMG_012_AbilityDamageReplacesStrengthOnHitEmpty()
    {
        // Arrange
        var abilityPower = new Power(
            "ability",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.ABILITY_DAMAGE, EffectTarget.OPPONENT, 3, null),
            null,
            null
        );
        
        var attackerCard = CreateCardWithPower("c1", "red", 5, "attack", "empty", "empty", "empty", abilityPower);
        var defenderCard = CreateCard("c2", "blue", 5, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(17, defender.Hp); // 20 - 3 = 17 (not 20 - 5)
    }

    [Fact]
    public void DMG_013_AbilityDamageHalvedOnHitAttack()
    {
        // Arrange
        var abilityPower = new Power(
            "ability",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.ABILITY_DAMAGE, EffectTarget.OPPONENT, 6, null),
            null,
            null
        );
        
        var attackerCard = CreateCardWithPower("c1", "red", 10, "attack", "empty", "empty", "empty", abilityPower);
        var defenderCard = CreateCard("c2", "blue", 5, "attack", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(17, defender.Hp); // 20 - floor(6/2) = 20 - 3 = 17
    }

    [Fact]
    public void DMG_014_WeakenDoesNotApplyToBurn()
    {
        // Arrange
        var player = new Player("p1", null!);
        player.AddStatus(new Status("weaken", 1, null, 100));
        player.AddStatus(new Status("burn", 1, 4, 101));
        
        // Act
        player.ResolveBurnAndRenew();
        
        // Assert
        Assert.Equal(16, player.Hp); // 20 - 4 = 16 (weaken doesn't apply)
    }

    [Fact]
    public void DMG_015_ProtectDoesNotApplyToBurn()
    {
        // Arrange
        var player = new Player("p1", null!);
        player.AddStatus(new Status("protect", 1, null, 100));
        player.AddStatus(new Status("burn", 1, 4, 101));
        
        // Act
        player.ResolveBurnAndRenew();
        
        // Assert
        Assert.Equal(16, player.Hp); // 20 - 4 = 16 (protect doesn't apply)
    }

    [Fact]
    public void DMG_016_VulnerableDoesNotApplyToBurn()
    {
        // Arrange
        var player = new Player("p1", null!);
        player.AddStatus(new Status("vulnerable", 1, null, 100));
        player.AddStatus(new Status("burn", 1, 4, 101));
        
        // Act
        player.ResolveBurnAndRenew();
        
        // Assert
        Assert.Equal(16, player.Hp); // 20 - 4 = 16 (vulnerable doesn't apply)
    }

    [Fact]
    public void DMG_017_WeaknessRedAppliesToRedCardStrengthDamage()
    {
        // Arrange
        var attackerCard = CreateCard("c1", "red", 4, "attack", "empty", "empty", "empty");
        var defenderCard = CreateCard("c2", "blue", 4, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        defender.AddStatus(new Status("weakness(red)", 1, null, 100));
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(12, defender.Hp); // 20 - (4 * 2) = 12
    }

    [Fact]
    public void DMG_018_WeaknessRedAppliesToRedCardAdditionalDamage()
    {
        // Arrange
        var additionalDamagePower = new Power(
            "additional",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.ADDITIONAL_DAMAGE, EffectTarget.OPPONENT, 3, null),
            null,
            null
        );
        
        var attackerCard = CreateCardWithPower("c1", "red", 0, "attack", "empty", "empty", "empty", additionalDamagePower);
        var defenderCard = CreateCard("c2", "blue", 5, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        defender.AddStatus(new Status("weakness(red)", 1, null, 100));
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(14, defender.Hp); // 20 - (3 * 2) = 14
    }

    [Fact]
    public void DMG_019_WeaknessRedDoesNotApplyToBlueCardDamage()
    {
        // Arrange
        var attackerCard = CreateCard("c1", "blue", 4, "attack", "empty", "empty", "empty");
        var defenderCard = CreateCard("c2", "red", 4, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        defender.AddStatus(new Status("weakness(red)", 1, null, 100));
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(16, defender.Hp); // 20 - 4 = 16 (no weakness multiplier)
    }

    [Fact]
    public void DMG_020_AllModifiersTogether()
    {
        // Arrange
        var attackerCard = CreateCard("c1", "red", 4, "attack", "empty", "empty", "empty");
        var defenderCard = CreateCard("c2", "blue", 4, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        attacker.AddStatus(new Status("weaken", 1, null, 100));
        defender.AddStatus(new Status("vulnerable", 1, null, 101));
        defender.AddStatus(new Status("weakness(red)", 1, null, 102));
        defender.AddStatus(new Status("protect", 1, null, 103));
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        // 4 * 0.5 (weaken) * 2 (vulnerable) * 2 (weakness) * 0.5 (protect) = 4
        Assert.Equal(16, defender.Hp); // 20 - 4 = 16
    }

    [Fact]
    public void DMG_021_AbilityDamageAndAdditionalDamageResolveInOrder()
    {
        // Arrange
        var abilityPower = new Power(
            "ability",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.ABILITY_DAMAGE, EffectTarget.OPPONENT, 3, null),
            null,
            null
        );
        
        var additionalDamagePower = new Power(
            "additional",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.ADDITIONAL_DAMAGE, EffectTarget.OPPONENT, 2, null),
            null,
            null
        );
        
        var attackerCard = CreateCardWithPowers("c1", "red", 10, "attack", "empty", "empty", "empty", 
            new[] { abilityPower, additionalDamagePower });
        var defenderCard = CreateCard("c2", "blue", 5, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(15, defender.Hp); // 20 - 3 (ability) - 2 (additional) = 15
    }

    // Helper methods
    private Card CreateCard(string id, string color, int strength, 
        string redState, string blueState, string greenState, string yellowState)
    {
        var zones = new[]
        {
            new Zone("red", redState),
            new Zone("blue", blueState),
            new Zone("green", greenState),
            new Zone("yellow", yellowState)
        };
        
        var dummyPower = new Power(
            "dummy",
            new Condition(ConditionType.AFTER_TURN_RESOLVES),
            new Effect(EffectType.HEAL, EffectTarget.SELF, 0, null),
            null,
            null
        );
        
        return new Card(id, "Test Card", color, strength, zones, new List<Power> { dummyPower });
    }

    private Card CreateCardWithPower(string id, string color, int strength,
        string redState, string blueState, string greenState, string yellowState, Power power)
    {
        var zones = new[]
        {
            new Zone("red", redState),
            new Zone("blue", blueState),
            new Zone("green", greenState),
            new Zone("yellow", yellowState)
        };
        
        return new Card(id, "Test Card", color, strength, zones, new List<Power> { power });
    }

    private Card CreateCardWithPowers(string id, string color, int strength,
        string redState, string blueState, string greenState, string yellowState, Power[] powers)
    {
        var zones = new[]
        {
            new Zone("red", redState),
            new Zone("blue", blueState),
            new Zone("green", greenState),
            new Zone("yellow", yellowState)
        };
        
        return new Card(id, "Test Card", color, strength, zones, new List<Power>(powers));
    }
}

// Made with Bob
