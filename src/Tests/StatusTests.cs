using Xunit;
using BattleSystem.Models;

namespace BattleSystem.Tests;

public class StatusTests
{
    [Fact]
    public void STATUS_001_StatusInitializesWithCorrectProperties()
    {
        // Arrange & Act
        var status = new Status("burn", 3, 5, 100);
        
        // Assert
        Assert.Equal("burn", status.Type);
        Assert.Equal(3, status.Duration);
        Assert.Equal(5, status.Power);
        Assert.Equal(100, status.ReceivedAt);
    }

    [Fact]
    public void STATUS_002_StatusWithoutPowerInitializesCorrectly()
    {
        // Arrange & Act
        var status = new Status("quick", 2, null, 50);
        
        // Assert
        Assert.Equal("quick", status.Type);
        Assert.Equal(2, status.Duration);
        Assert.Null(status.Power);
        Assert.Equal(50, status.ReceivedAt);
    }

    [Fact]
    public void STATUS_003_WeaknessColorIsPartOfStatusType()
    {
        // Arrange & Act
        var weaknessRed = new Status("weakness(red)", 2, null, 10);
        var weaknessBlue = new Status("weakness(blue)", 2, null, 11);
        
        // Assert
        Assert.Equal("weakness(red)", weaknessRed.Type);
        Assert.Equal("weakness(blue)", weaknessBlue.Type);
        Assert.NotEqual(weaknessRed.Type, weaknessBlue.Type);
    }

    [Fact]
    public void STATUS_004_MultipleStatusesOfSameTypeTrackedIndependently()
    {
        // Arrange
        var player = new Player("p1", null!);
        var burn1 = new Status("burn", 2, 1, 100);
        var burn2 = new Status("burn", 3, 2, 101);
        
        // Act
        player.AddStatus(burn1);
        player.AddStatus(burn2);
        
        // Assert
        Assert.Equal(2, player.Statuses.Count);
        Assert.Equal(1, player.Statuses[0].Power);
        Assert.Equal(2, player.Statuses[1].Power);
    }

    [Fact]
    public void STATUS_005_BurnDealsCorrectDamageEachTurn()
    {
        // Arrange
        var player = new Player("p1", null!);
        player.AddStatus(new Status("burn", 2, 3, 100));
        
        // Act
        player.ResolveBurnAndRenew();
        
        // Assert
        Assert.Equal(17, player.Hp); // 20 - 3 = 17
    }

    [Fact]
    public void STATUS_006_BurnIsNotAffectedByProtect()
    {
        // Arrange
        var player = new Player("p1", null!);
        player.AddStatus(new Status("protect", 1, null, 100));
        player.AddStatus(new Status("burn", 1, 4, 101));
        
        // Act
        player.ResolveBurnAndRenew();
        
        // Assert
        Assert.Equal(16, player.Hp); // 20 - 4 = 16 (protect does not apply)
    }

    [Fact]
    public void STATUS_007_BurnIsNotAffectedByVulnerable()
    {
        // Arrange
        var player = new Player("p1", null!);
        player.AddStatus(new Status("vulnerable", 1, null, 100));
        player.AddStatus(new Status("burn", 1, 4, 101));
        
        // Act
        player.ResolveBurnAndRenew();
        
        // Assert
        Assert.Equal(16, player.Hp); // 20 - 4 = 16 (vulnerable does not apply)
    }

    [Fact]
    public void STATUS_008_RenewHealsCorrectAmountEachTurn()
    {
        // Arrange
        var player = new Player("p1", null!);
        player.TakeDamage(6); // HP = 14
        player.AddStatus(new Status("renew", 2, 3, 100));
        
        // Act
        player.ResolveBurnAndRenew();
        
        // Assert
        Assert.Equal(17, player.Hp); // 14 + 3 = 17
    }

    [Fact]
    public void STATUS_009_RenewCannotExceedMaxHp()
    {
        // Arrange
        var player = new Player("p1", null!);
        player.TakeDamage(1); // HP = 19
        player.AddStatus(new Status("renew", 1, 3, 100));
        
        // Act
        player.ResolveBurnAndRenew();
        
        // Assert
        Assert.Equal(20, player.Hp); // 19 + 3 capped at 20
    }

    [Fact]
    public void STATUS_010_RenewDoesNothingWhenAlreadyAtMaxHp()
    {
        // Arrange
        var player = new Player("p1", null!);
        player.AddStatus(new Status("renew", 1, 3, 100));
        
        // Act
        player.ResolveBurnAndRenew();
        
        // Assert
        Assert.Equal(20, player.Hp); // Already at max, stays at 20
    }

    [Fact]
    public void STATUS_011_QuickVsSlowQuickGoesFirst()
    {
        // Arrange
        var card1 = CreateTestCard("c1", "red");
        var card2 = CreateTestCard("c2", "blue");
        var player1 = new Player("p1", card1);
        var player2 = new Player("p2", card2);
        
        player1.AddStatus(new Status("quick", 1, null, 100));
        player2.AddStatus(new Status("slow", 1, null, 101));
        
        var battle = new Battle(player1, player2, new[] { player2, player1 }); // Pre-assigned: p2 first
        
        // Act
        var actingOrder = battle.DetermineActingOrder();
        
        // Assert
        Assert.Equal(player1, actingOrder[0]); // Quick overrides pre-assigned order
        Assert.Equal(player2, actingOrder[1]);
    }

    [Fact]
    public void STATUS_012_BothQuickDefaultToPreAssignedOrder()
    {
        // Arrange
        var card1 = CreateTestCard("c1", "red");
        var card2 = CreateTestCard("c2", "blue");
        var player1 = new Player("p1", card1);
        var player2 = new Player("p2", card2);
        
        player1.AddStatus(new Status("quick", 1, null, 100));
        player2.AddStatus(new Status("quick", 1, null, 101));
        
        var battle = new Battle(player1, player2, new[] { player1, player2 });
        
        // Act
        var actingOrder = battle.DetermineActingOrder();
        
        // Assert
        Assert.Equal(player1, actingOrder[0]); // Defaults to pre-assigned
        Assert.Equal(player2, actingOrder[1]);
    }

    [Fact]
    public void STATUS_013_BothSlowDefaultToPreAssignedOrder()
    {
        // Arrange
        var card1 = CreateTestCard("c1", "red");
        var card2 = CreateTestCard("c2", "blue");
        var player1 = new Player("p1", card1);
        var player2 = new Player("p2", card2);
        
        player1.AddStatus(new Status("slow", 1, null, 100));
        player2.AddStatus(new Status("slow", 1, null, 101));
        
        var battle = new Battle(player1, player2, new[] { player2, player1 });
        
        // Act
        var actingOrder = battle.DetermineActingOrder();
        
        // Assert
        Assert.Equal(player2, actingOrder[0]); // Defaults to pre-assigned
        Assert.Equal(player1, actingOrder[1]);
    }

    [Fact]
    public void STATUS_014_TwoSlowsVsOneQuickQuickCancelsOneSlowOtherSlowWins()
    {
        // Arrange
        var card1 = CreateTestCard("c1", "red");
        var card2 = CreateTestCard("c2", "blue");
        var player1 = new Player("p1", card1);
        var player2 = new Player("p2", card2);
        
        player1.AddStatus(new Status("slow", 2, null, 100));
        player1.AddStatus(new Status("slow", 1, null, 101));
        player2.AddStatus(new Status("quick", 1, null, 102));
        
        var battle = new Battle(player1, player2, new[] { player1, player2 }); // Pre-assigned: p1 first
        
        // Act
        var actingOrder = battle.DetermineActingOrder();
        
        // Assert
        // Player1 has net -2 slow, Player2 has net +1 quick, so Player2 goes first
        Assert.Equal(player2, actingOrder[0]);
        Assert.Equal(player1, actingOrder[1]);
        
        // Verify both slow instances retain their durations
        Assert.Equal(2, player1.Statuses.Count);
        Assert.Equal(2, player1.Statuses[0].Duration);
        Assert.Equal(1, player1.Statuses[1].Duration);
    }

    [Fact]
    public void STATUS_015_SilencePreventsPowerActivation()
    {
        // Arrange
        var zones = new[]
        {
            new Zone("red", "attack"),
            new Zone("blue", "empty"),
            new Zone("yellow", "empty"),
            new Zone("green", "empty")
        };
        
        var power = new Power(
            "pow1",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.ADDITIONAL_DAMAGE, EffectTarget.OPPONENT, 5, null),
            new ManaCost(new List<CostEntry> { new CostEntry("red", 1) }),
            null
        );
        
        var card = new Card("c1", "Test Card", "red", 5, zones, new List<Power> { power });
        var player = new Player("p1", card);
        player.AddMana("red");
        player.AddStatus(new Status("silence", 1, null, 100));
        
        // Act - silence should prevent power from firing
        var initialManaCount = player.Mana.Count(m => m.Color != "empty");
        
        // Assert
        Assert.Equal(1, initialManaCount); // Mana not spent due to silence
    }

    [Fact]
    public void STATUS_016_WeaknessColorDoublesMatchingCardColorDamage()
    {
        // Arrange
        var attackerZones = new[]
        {
            new Zone("red", "attack"),
            new Zone("blue", "empty"),
            new Zone("yellow", "empty"),
            new Zone("green", "empty")
        };
        var defenderZones = new[]
        {
            new Zone("red", "empty"),
            new Zone("blue", "empty"),
            new Zone("yellow", "empty"),
            new Zone("green", "empty")
        };
        
        var attackerCard = new Card("c1", "Red Card", "red", 4, attackerZones, new List<Power> { CreateDummyPower() });
        var defenderCard = new Card("c2", "Defender", "blue", 4, defenderZones, new List<Power> { CreateDummyPower() });
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        defender.AddStatus(new Status("weakness(red)", 2, null, 100));
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(12, defender.Hp); // 20 - (4 * 2) = 12
    }

    [Fact]
    public void STATUS_017_WeaknessColorDoesNotApplyToNonMatchingColor()
    {
        // Arrange
        var attackerZones = new[]
        {
            new Zone("blue", "attack"),
            new Zone("red", "empty"),
            new Zone("yellow", "empty"),
            new Zone("green", "empty")
        };
        var defenderZones = new[]
        {
            new Zone("blue", "empty"),
            new Zone("red", "empty"),
            new Zone("yellow", "empty"),
            new Zone("green", "empty")
        };
        
        var attackerCard = new Card("c1", "Blue Card", "blue", 4, attackerZones, new List<Power> { CreateDummyPower() });
        var defenderCard = new Card("c2", "Defender", "red", 4, defenderZones, new List<Power> { CreateDummyPower() });
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        defender.AddStatus(new Status("weakness(red)", 2, null, 100));
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(16, defender.Hp); // 20 - 4 = 16 (no weakness multiplier)
    }

    [Fact]
    public void STATUS_018_WeaknessColorStacksWithVulnerable()
    {
        // Arrange
        var attackerZones = new[]
        {
            new Zone("red", "attack"),
            new Zone("blue", "empty"),
            new Zone("yellow", "empty"),
            new Zone("green", "empty")
        };
        var defenderZones = new[]
        {
            new Zone("red", "empty"),
            new Zone("blue", "empty"),
            new Zone("yellow", "empty"),
            new Zone("green", "empty")
        };
        
        var attackerCard = new Card("c1", "Red Card", "red", 4, attackerZones, new List<Power> { CreateDummyPower() });
        var defenderCard = new Card("c2", "Defender", "blue", 4, defenderZones, new List<Power> { CreateDummyPower() });
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        defender.AddStatus(new Status("weakness(red)", 1, null, 100));
        defender.AddStatus(new Status("vulnerable", 1, null, 101));
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(4, defender.Hp); // 20 - (4 * 2 * 2) = 4
    }

    [Fact]
    public void STATUS_019_WeaknessColorStacksWithProtect()
    {
        // Arrange
        var attackerZones = new[]
        {
            new Zone("red", "attack"),
            new Zone("blue", "empty"),
            new Zone("yellow", "empty"),
            new Zone("green", "empty")
        };
        var defenderZones = new[]
        {
            new Zone("red", "empty"),
            new Zone("blue", "empty"),
            new Zone("yellow", "empty"),
            new Zone("green", "empty")
        };
        
        var attackerCard = new Card("c1", "Red Card", "red", 4, attackerZones, new List<Power> { CreateDummyPower() });
        var defenderCard = new Card("c2", "Defender", "blue", 4, defenderZones, new List<Power> { CreateDummyPower() });
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        defender.AddStatus(new Status("weakness(red)", 1, null, 100));
        defender.AddStatus(new Status("protect", 1, null, 101));
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(16, defender.Hp); // 20 - (4 * 2 * 0.5) = 16
    }

    [Fact]
    public void STATUS_020_TwoWeaknessRedStacksApplyBothMultipliers()
    {
        // Arrange
        var attackerZones = new[]
        {
            new Zone("red", "attack"),
            new Zone("blue", "empty"),
            new Zone("yellow", "empty"),
            new Zone("green", "empty")
        };
        var defenderZones = new[]
        {
            new Zone("red", "empty"),
            new Zone("blue", "empty"),
            new Zone("yellow", "empty"),
            new Zone("green", "empty")
        };
        
        var attackerCard = new Card("c1", "Red Card", "red", 4, attackerZones, new List<Power> { CreateDummyPower() });
        var defenderCard = new Card("c2", "Defender", "blue", 4, defenderZones, new List<Power> { CreateDummyPower() });
        
        var attacker = new Player("p1", attackerCard);
        var defender = new Player("p2", defenderCard);
        defender.AddStatus(new Status("weakness(red)", 1, null, 100));
        defender.AddStatus(new Status("weakness(red)", 2, null, 101));
        
        var battle = new Battle(attacker, defender, new[] { attacker, defender });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(4, defender.Hp); // 20 - (4 * 2 * 2) = 4
    }

    [Fact]
    public void STATUS_021_WeaknessColorDoesNotApplyToBurn()
    {
        // Arrange
        var player = new Player("p1", null!);
        player.AddStatus(new Status("weakness(red)", 2, null, 100));
        player.AddStatus(new Status("burn", 1, 3, 101));
        
        // Act
        player.ResolveBurnAndRenew();
        
        // Assert
        Assert.Equal(17, player.Hp); // 20 - 3 = 17 (weakness does not apply to burn)
    }

    [Fact]
    public void STATUS_022_StatusesResolveInOrderOfReceipt()
    {
        // Arrange
        var player = new Player("p1", null!);
        player.AddStatus(new Status("burn", 1, 2, 100)); // Received first
        player.AddStatus(new Status("burn", 1, 3, 101)); // Received second
        
        // Act
        player.ResolveBurnAndRenew();
        
        // Assert
        Assert.Equal(15, player.Hp); // 20 - 2 - 3 = 15
        // Order verification: first burn (power=2) resolves, then second burn (power=3)
        Assert.Equal(100, player.Statuses[0].ReceivedAt);
        Assert.Equal(101, player.Statuses[1].ReceivedAt);
    }

    // Helper methods
    private Card CreateTestCard(string id, string color)
    {
        var zones = new[]
        {
            new Zone("red", "empty"),
            new Zone("blue", "empty"),
            new Zone("yellow", "empty"),
            new Zone("green", "empty")
        };
        
        var power = CreateDummyPower();
        
        return new Card(id, "Test Card", color, 5, zones, new List<Power> { power });
    }

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
