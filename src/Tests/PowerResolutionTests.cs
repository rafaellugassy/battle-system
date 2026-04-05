using Xunit;
using BattleSystem.Models;

namespace BattleSystem.Tests;

public class PowerResolutionTests
{
    [Fact]
    public void PWR_001_OnHitPowerFiresOnHitEmpty()
    {
        // Arrange
        var burnStatus = new Status("burn", 2, 3, 100);
        var onHitPower = new Power(
            "onhit",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.APPLY_STATUS, EffectTarget.OPPONENT, null, burnStatus),
            null,
            null
        );
        
        var attackerCard = CreateCardWithPower("c1", "red", 5, "attack", "empty", "empty", "empty", onHitPower);
        var defenderCard = CreateCard("c2", "blue", 5, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.True(defender.HasStatus("burn"));
    }

    [Fact]
    public void PWR_002_OnHitPowerFiresOnHitAttack()
    {
        // Arrange
        var burnStatus = new Status("burn", 2, 3, 100);
        var onHitPower = new Power(
            "onhit",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.APPLY_STATUS, EffectTarget.OPPONENT, null, burnStatus),
            null,
            null
        );
        
        var attackerCard = CreateCardWithPower("c1", "red", 5, "attack", "empty", "empty", "empty", onHitPower);
        var defenderCard = CreateCard("c2", "blue", 5, "attack", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.True(defender.HasStatus("burn"));
    }

    [Fact]
    public void PWR_003_OnHitPowerDoesNotFireOnNoHit()
    {
        // Arrange
        var burnStatus = new Status("burn", 2, 3, 100);
        var onHitPower = new Power(
            "onhit",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.APPLY_STATUS, EffectTarget.OPPONENT, null, burnStatus),
            null,
            null
        );
        
        var attackerCard = CreateCardWithPower("c1", "red", 5, "attack", "empty", "empty", "empty", onHitPower);
        var defenderCard = CreateCard("c2", "blue", 5, "defend", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.False(defender.HasStatus("burn"));
    }

    [Fact]
    public void PWR_004_OnDefendPowerFiresWhenDefendSuccessfullyBlocks()
    {
        // Arrange
        var slowStatus = new Status("slow", 2, null, 100);
        var onDefendPower = new Power(
            "ondefend",
            new Condition(ConditionType.ON_DEFEND),
            new Effect(EffectType.APPLY_STATUS, EffectTarget.OPPONENT, null, slowStatus),
            null,
            null
        );
        
        var attackerCard = CreateCard("c1", "red", 5, "attack", "empty", "empty", "empty");
        var defenderCard = CreateCardWithPower("c2", "blue", 5, "defend", "empty", "empty", "empty", onDefendPower);
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.True(attacker.HasStatus("slow"));
    }

    [Fact]
    public void PWR_005_OnDefendPowerDoesNotFireWhenDefendZoneNotAttacked()
    {
        // Arrange
        var slowStatus = new Status("slow", 2, null, 100);
        var onDefendPower = new Power(
            "ondefend",
            new Condition(ConditionType.ON_DEFEND),
            new Effect(EffectType.APPLY_STATUS, EffectTarget.OPPONENT, null, slowStatus),
            null,
            null
        );
        
        var attackerCard = CreateCard("c1", "red", 5, "empty", "empty", "empty", "empty");
        var defenderCard = CreateCardWithPower("c2", "blue", 5, "defend", "empty", "empty", "empty", onDefendPower);
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.False(attacker.HasStatus("slow"));
    }

    [Fact]
    public void PWR_006_OnBeingHitFiresWhenThisCardsZoneIsHit()
    {
        // Arrange
        var vulnerableStatus = new Status("vulnerable", 2, null, 100);
        var onBeingHitPower = new Power(
            "onbeinghit",
            new Condition(ConditionType.ON_BEING_HIT),
            new Effect(EffectType.APPLY_STATUS, EffectTarget.OPPONENT, null, vulnerableStatus),
            null,
            null
        );
        
        var attackerCard = CreateCard("c1", "red", 5, "attack", "empty", "empty", "empty");
        var defenderCard = CreateCardWithPower("c2", "blue", 5, "empty", "empty", "empty", "empty", onBeingHitPower);
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.True(attacker.HasStatus("vulnerable"));
    }

    [Fact]
    public void PWR_007_OnBeingHitFiresWhenAttackVsAttack()
    {
        // Arrange
        var vulnerableStatus = new Status("vulnerable", 2, null, 100);
        var onBeingHitPower = new Power(
            "onbeinghit",
            new Condition(ConditionType.ON_BEING_HIT),
            new Effect(EffectType.APPLY_STATUS, EffectTarget.OPPONENT, null, vulnerableStatus),
            null,
            null
        );
        
        var attackerCard = CreateCard("c1", "red", 5, "attack", "empty", "empty", "empty");
        var defenderCard = CreateCardWithPower("c2", "blue", 5, "attack", "empty", "empty", "empty", onBeingHitPower);
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.True(attacker.HasStatus("vulnerable"));
    }

    [Fact]
    public void PWR_008_OnBeingHitDoesNotFireWhenThisCardZoneBlocks()
    {
        // Arrange
        var vulnerableStatus = new Status("vulnerable", 2, null, 100);
        var onBeingHitPower = new Power(
            "onbeinghit",
            new Condition(ConditionType.ON_BEING_HIT),
            new Effect(EffectType.APPLY_STATUS, EffectTarget.OPPONENT, null, vulnerableStatus),
            null,
            null
        );
        
        var attackerCard = CreateCard("c1", "red", 5, "attack", "empty", "empty", "empty");
        var defenderCard = CreateCardWithPower("c2", "blue", 5, "defend", "empty", "empty", "empty", onBeingHitPower);
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.False(attacker.HasStatus("vulnerable"));
    }

    [Fact]
    public void PWR_009_AfterTurnResolvesFiresAfterAllZoneResolution()
    {
        // Arrange
        var afterTurnPower = new Power(
            "afterturn",
            new Condition(ConditionType.AFTER_TURN_RESOLVES),
            new Effect(EffectType.ADDITIONAL_DAMAGE, EffectTarget.BOTH, 2, null),
            null,
            null
        );
        
        var attackerCard = CreateCardWithPower("c1", "red", 0, "empty", "empty", "empty", "empty", afterTurnPower);
        var defenderCard = CreateCard("c2", "blue", 0, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(18, attacker.Hp); // 20 - 2 = 18
        Assert.Equal(18, defender.Hp); // 20 - 2 = 18
    }

    [Fact]
    public void PWR_010_ManaCostIsSpentWhenPowerActivates()
    {
        // Arrange
        var burnStatus = new Status("burn", 2, 2, 100);
        var onHitPower = new Power(
            "onhit",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.APPLY_STATUS, EffectTarget.OPPONENT, null, burnStatus),
            new ManaCost(new List<CostEntry> { new CostEntry("blue", 1) }),
            null
        );
        
        var attackerCard = CreateCardWithPower("c1", "red", 5, "attack", "empty", "empty", "empty", onHitPower);
        var defenderCard = CreateCard("c2", "blue", 5, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        attacker.AddMana("blue"); // receivedAt = 1
        attacker.AddMana("blue"); // receivedAt = 2
        attacker.AddMana("red");  // receivedAt = 3
        
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.True(defender.HasStatus("burn")); // Power activated
        var remainingMana = attacker.Mana.Where(m => m.Color != "empty").ToList();
        Assert.Equal(3, remainingMana.Count); // 3 - 1 (spent) + 1 (generated in step 8) = 3
        // Check we have 1 blue and 2 red (order may vary)
        Assert.Equal(1, remainingMana.Count(m => m.Color == "blue")); // Second blue remains
        Assert.Equal(2, remainingMana.Count(m => m.Color == "red")); // Original red + generated red
    }

    [Fact]
    public void PWR_011_PowerDoesNotActivateIfManaCostNotMet()
    {
        // Arrange
        var burnStatus = new Status("burn", 2, 2, 100);
        var onHitPower = new Power(
            "onhit",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.APPLY_STATUS, EffectTarget.OPPONENT, null, burnStatus),
            new ManaCost(new List<CostEntry> { new CostEntry("blue", 1) }),
            null
        );
        
        var attackerCard = CreateCardWithPower("c1", "red", 5, "attack", "empty", "empty", "empty", onHitPower);
        var defenderCard = CreateCard("c2", "blue", 5, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        attacker.AddMana("red"); // Only red mana, need blue
        
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.False(defender.HasStatus("burn")); // Power did not activate
        Assert.Equal(2, attacker.Mana.Count(m => m.Color != "empty")); // 1 red + 1 red generated in step 8
    }

    [Fact]
    public void PWR_012_ManaRequirementIsCheckedButNotSpent()
    {
        // Arrange
        var burnStatus = new Status("burn", 2, 2, 100);
        var onHitPower = new Power(
            "onhit",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.APPLY_STATUS, EffectTarget.OPPONENT, null, burnStatus),
            null,
            new ManaRequirement(new List<RequirementEntry> { new RequirementEntry("blue", 2) })
        );
        
        var attackerCard = CreateCardWithPower("c1", "red", 5, "attack", "empty", "empty", "empty", onHitPower);
        var defenderCard = CreateCard("c2", "blue", 5, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        attacker.AddMana("blue");
        attacker.AddMana("blue");
        
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.True(defender.HasStatus("burn")); // Power activated
        Assert.Equal(3, attacker.Mana.Count(m => m.Color != "empty")); // 2 blue (not spent) + 1 red (generated in step 8)
    }

    [Fact]
    public void PWR_013_PowerDoesNotActivateIfManaRequirementNotMet()
    {
        // Arrange
        var burnStatus = new Status("burn", 2, 2, 100);
        var onHitPower = new Power(
            "onhit",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.APPLY_STATUS, EffectTarget.OPPONENT, null, burnStatus),
            null,
            new ManaRequirement(new List<RequirementEntry> { new RequirementEntry("blue", 2) })
        );
        
        var attackerCard = CreateCardWithPower("c1", "red", 5, "attack", "empty", "empty", "empty", onHitPower);
        var defenderCard = CreateCard("c2", "blue", 5, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        attacker.AddMana("blue"); // Only 1 blue, need 2
        
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.False(defender.HasStatus("burn")); // Power did not activate
    }

    [Fact]
    public void PWR_014_PowerWithBothCostAndRequirementBothMustBeSatisfied()
    {
        // Arrange
        var burnStatus = new Status("burn", 2, 2, 100);
        var onHitPower = new Power(
            "onhit",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.APPLY_STATUS, EffectTarget.OPPONENT, null, burnStatus),
            new ManaCost(new List<CostEntry> { new CostEntry("red", 1) }),
            new ManaRequirement(new List<RequirementEntry> { new RequirementEntry("blue", 2) })
        );
        
        var attackerCard = CreateCardWithPower("c1", "red", 5, "attack", "empty", "empty", "empty", onHitPower);
        var defenderCard = CreateCard("c2", "blue", 5, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        attacker.AddMana("blue");
        attacker.AddMana("blue");
        attacker.AddMana("red");
        
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.True(defender.HasStatus("burn")); // Power activated
        var remainingMana = attacker.Mana.Where(m => m.Color != "empty").ToList();
        Assert.Equal(3, remainingMana.Count); // 1 red consumed, 2 blues remain, 1 red generated in step 8
        Assert.Equal("blue", remainingMana[0].Color);
        Assert.Equal("blue", remainingMana[1].Color);
        Assert.Equal("red", remainingMana[2].Color); // Generated in step 8
    }

    [Fact]
    public void PWR_015_PowerWithBothCostAndRequirementRequirementNotMetEvenIfCostCanBePaid()
    {
        // Arrange
        var burnStatus = new Status("burn", 2, 2, 100);
        var onHitPower = new Power(
            "onhit",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.APPLY_STATUS, EffectTarget.OPPONENT, null, burnStatus),
            new ManaCost(new List<CostEntry> { new CostEntry("red", 1) }),
            new ManaRequirement(new List<RequirementEntry> { new RequirementEntry("blue", 2) })
        );
        
        var attackerCard = CreateCardWithPower("c1", "red", 5, "attack", "empty", "empty", "empty", onHitPower);
        var defenderCard = CreateCard("c2", "blue", 5, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        attacker.AddMana("blue"); // Only 1 blue, need 2
        attacker.AddMana("red");
        attacker.AddMana("red");
        
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.False(defender.HasStatus("burn")); // Power did not activate
        Assert.Equal(4, attacker.Mana.Count(m => m.Color != "empty")); // 3 original + 1 red generated in step 8
    }

    [Fact]
    public void PWR_016_HitTwiceCausesOnHitToFireTwice()
    {
        // Arrange
        var slowStatus1 = new Status("slow", 2, null, 100);
        var slowStatus2 = new Status("slow", 2, null, 101);
        
        var hitTwicePower = new Power(
            "hittwice",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.HIT_TWICE, EffectTarget.SELF, null, null),
            null,
            null
        );
        
        var applyStatusPower = new Power(
            "applystatus",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.APPLY_STATUS, EffectTarget.OPPONENT, null, slowStatus1),
            null,
            null
        );
        
        var attackerCard = CreateCardWithPowers("c1", "red", 5, "attack", "empty", "empty", "empty", 
            new[] { hitTwicePower, applyStatusPower });
        var defenderCard = CreateCard("c2", "blue", 5, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(2, defender.GetStatusCount("slow")); // Status applied twice
    }

    [Fact]
    public void PWR_017_HitTwiceBothHitsResolveOnHitAttackBothHalved()
    {
        // Arrange
        var hitTwicePower = new Power(
            "hittwice",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.HIT_TWICE, EffectTarget.SELF, null, null),
            null,
            null
        );
        
        var attackerCard = CreateCardWithPower("c1", "red", 4, "attack", "empty", "empty", "empty", hitTwicePower);
        var defenderCard = CreateCard("c2", "blue", 4, "attack", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        // HIT_ATTACK: each hit is halved (floor(4/2) = 2), total = 2 + 2 = 4
        Assert.Equal(16, defender.Hp); // 20 - 4 = 16
    }

    [Fact]
    public void PWR_018_HitTwiceBothHitsResolveOnHitEmptyBothFull()
    {
        // Arrange
        var hitTwicePower = new Power(
            "hittwice",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.HIT_TWICE, EffectTarget.SELF, null, null),
            null,
            null
        );
        
        var attackerCard = CreateCardWithPower("c1", "red", 4, "attack", "empty", "empty", "empty", hitTwicePower);
        var defenderCard = CreateCard("c2", "blue", 4, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        // HIT_EMPTY: each hit is full (4), total = 4 + 4 = 8
        Assert.Equal(12, defender.Hp); // 20 - 8 = 12
    }

    [Fact]
    public void PWR_019_HitTwiceFirstHitEliminatesOpponentSecondHitStillResolves()
    {
        // Arrange
        var hitTwicePower = new Power(
            "hittwice",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.HIT_TWICE, EffectTarget.SELF, null, null),
            null,
            null
        );
        
        var attackerCard = CreateCardWithPower("c1", "red", 4, "attack", "empty", "empty", "empty", hitTwicePower);
        var defenderCard = CreateCard("c2", "blue", 4, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        defender.TakeDamage(17); // Set HP to 3
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.True(defender.IsEliminated);
        Assert.True(defender.Hp <= 0); // First hit: 3 - 4 = -1, second hit continues
        Assert.Equal("player1", battle.Result);
    }

    [Fact]
    public void PWR_020_SilencePreventsAllPowersFromFiring()
    {
        // Arrange
        var burnStatus = new Status("burn", 2, 2, 100);
        var onHitPower = new Power(
            "onhit",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.APPLY_STATUS, EffectTarget.OPPONENT, null, burnStatus),
            new ManaCost(new List<CostEntry> { new CostEntry("red", 1) }),
            null
        );
        
        var attackerCard = CreateCardWithPower("c1", "red", 5, "attack", "empty", "empty", "empty", onHitPower);
        var defenderCard = CreateCard("c2", "blue", 5, "empty", "empty", "empty", "empty");
        
        var attacker = new Player("p1", attackerCard);
        attacker.AddMana("red");
        attacker.AddStatus(new Status("silence", 1, null, 100));
        
        var defender = new Player("p2", defenderCard);
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.False(defender.HasStatus("burn")); // Power did not fire
        Assert.Equal(2, attacker.Mana.Count(m => m.Color != "empty")); // 1 red (not spent) + 1 red (generated in step 8)
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
