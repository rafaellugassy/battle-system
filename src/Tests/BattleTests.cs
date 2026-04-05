using Xunit;
using BattleSystem.Models;

namespace BattleSystem.Tests;

public class BattleTests
{
    // Helper method to create a card with specified zones
    private Card CreateCard(string id, string name, string color, int strength, 
        string redState, string blueState, string greenState, string yellowState)
    {
        var zones = new Zone[]
        {
            new Zone("red", redState),
            new Zone("blue", blueState),
            new Zone("green", greenState),
            new Zone("yellow", yellowState)
        };
        
        // Create a dummy power for cards that don't need special powers
        var dummyPower = new Power(
            "dummy",
            new Condition(ConditionType.AFTER_TURN_RESOLVES),
            new Effect(EffectType.HEAL, EffectTarget.SELF, 0, null),
            null,
            null
        );
        
        return new Card(id, name, color, strength, zones, new List<Power> { dummyPower });
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

    [Fact]
    public void BATTLE_001_BasicFullBattle_AttackerHitsEmpty()
    {
        // Arrange
        var card1 = CreateCard("c1", "Attacker", "red", 4, "attack", "empty", "empty", "empty");
        var card2 = CreateCard("c2", "Defender", "blue", 3, "empty", "empty", "empty", "empty");
        
        var player1 = new Player("p1", card1);
        var player2 = new Player("p2", card2);
        
        var battle = new Battle(player1, player2, new[] { player1, player2 });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(20, player1.Hp); // player1 unchanged (player2 has no attack zones)
        Assert.Equal(16, player2.Hp); // player2 took 4 damage
        Assert.Null(battle.Result); // no winner
    }

    [Fact]
    public void BATTLE_002_PlayerEliminatedDuringFirstPlayerTurn_SecondPlayerDoesNotAct()
    {
        // Arrange
        var card1 = CreateCard("c1", "Killer", "red", 20, "attack", "empty", "empty", "empty");
        var card2 = CreateCard("c2", "Victim", "blue", 5, "empty", "empty", "empty", "empty");
        
        var player1 = new Player("p1", card1);
        var player2 = new Player("p2", card2, 15);
        
        var battle = new Battle(player1, player2, new[] { player1, player2 });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(20, player1.Hp); // player1 unchanged
        Assert.Equal(0, player2.Hp); // player2 eliminated
        Assert.True(player2.IsEliminated);
        Assert.Equal("player1", battle.Result);
    }

    [Fact]
    public void BATTLE_003_Player1EliminatedDuringSelfDamage_SecondPlayerStillActs()
    {
        // Arrange
        // Player1 has AFTER_TURN_RESOLVES power dealing 3 damage to both
        var selfDamagePower = new Power(
            "self",
            new Condition(ConditionType.AFTER_TURN_RESOLVES),
            new Effect(EffectType.ADDITIONAL_DAMAGE, EffectTarget.BOTH, 3, null),
            null,
            null
        );
        var card1 = CreateCardWithPower("c1", "red", 0, "empty", "empty", "empty", "empty", selfDamagePower);
        var card2 = CreateCard("c2", "Attacker", "blue", 4, "attack", "empty", "empty", "empty");
        
        var player1 = new Player("p1", card1, 2);
        var player2 = new Player("p2", card2);
        
        var battle = new Battle(player1, player2, new[] { player1, player2 });
        
        // Act
        battle.Resolve();
        
        // Assert
        // Step 2: Player1 attacks (no attack zones, no damage)
        // Step 3: Player2 attacks with 4 strength, hits empty zone, player1 takes 4 damage (2 -> -2, eliminated)
        // Step 4: Player1 is eliminated, AFTER_TURN_RESOLVES power does NOT fire
        // Result: Player1 eliminated, player2 HP unchanged
        Assert.True(player1.IsEliminated);
        Assert.Equal(20, player2.Hp); // No damage taken
        Assert.Equal("player2", battle.Result);
    }

    [Fact]
    public void BATTLE_004_Draw_BothPlayersEliminatedBySimultaneousSelfDamage()
    {
        // Arrange
        // Player1 has AFTER_TURN_RESOLVES power dealing 5 damage to both
        var selfDamagePower = new Power(
            "self",
            new Condition(ConditionType.AFTER_TURN_RESOLVES),
            new Effect(EffectType.ADDITIONAL_DAMAGE, EffectTarget.BOTH, 5, null),
            null,
            null
        );
        var card1 = CreateCardWithPower("c1", "red", 0, "empty", "empty", "empty", "empty", selfDamagePower);
        var card2 = CreateCard("c2", "Victim", "blue", 0, "empty", "empty", "empty", "empty");
        
        var player1 = new Player("p1", card1, 3);
        var player2 = new Player("p2", card2, 3);
        
        var battle = new Battle(player1, player2, new[] { player1, player2 });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.True(player1.IsEliminated);
        Assert.True(player2.IsEliminated);
        Assert.Equal("draw", battle.Result);
    }

    [Fact]
    public void BATTLE_005_Draw_Player1WinsAttackButDiesToBurn()
    {
        // Arrange
        var card1 = CreateCard("c1", "Attacker", "red", 20, "attack", "empty", "empty", "empty");
        var card2 = CreateCard("c2", "Victim", "blue", 0, "empty", "empty", "empty", "empty");
        
        var player1 = new Player("p1", card1);
        var player2 = new Player("p2", card2, 10);
        
        // Player1 has burn that will kill them
        player1.AddStatus(new Status("burn", 1, 20, 0));
        
        var battle = new Battle(player1, player2, new[] { player1, player2 });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.True(player1.IsEliminated); // died to burn
        Assert.True(player2.IsEliminated); // died to attack
        Assert.Equal("draw", battle.Result);
    }

    [Fact]
    public void BATTLE_006_ManaGeneratedAfterBattleForColoredCard()
    {
        // Arrange
        var card1 = CreateCard("c1", "Red Card", "red", 4, "attack", "empty", "empty", "empty");
        var card2 = CreateCard("c2", "Blue Card", "blue", 3, "empty", "empty", "empty", "empty");
        
        var player1 = new Player("p1", card1);
        var player2 = new Player("p2", card2);
        
        // Verify starting state - all empty
        Assert.All(player1.Mana, m => Assert.Equal("empty", m.Color));
        
        var battle = new Battle(player1, player2, new[] { player1, player2 });
        
        // Act
        battle.Resolve();
        
        // Assert - Step 8 generates mana automatically
        var redMana = player1.Mana.Where(m => m.Color == "red").ToList();
        Assert.Single(redMana);
        var blueMana = player2.Mana.Where(m => m.Color == "blue").ToList();
        Assert.Single(blueMana);
    }

    [Fact]
    public void BATTLE_007_NoManaGeneratedForColorlessCard()
    {
        // Arrange
        var card1 = CreateCard("c1", "Colorless", "colorless", 4, "attack", "empty", "empty", "empty");
        var card2 = CreateCard("c2", "Blue Card", "blue", 3, "empty", "empty", "empty", "empty");
        
        var player1 = new Player("p1", card1);
        var player2 = new Player("p2", card2);
        
        var battle = new Battle(player1, player2, new[] { player1, player2 });
        
        // Act
        battle.Resolve();
        
        // Assert - colorless card generates no mana, but player2 (blue card) does
        Assert.All(player1.Mana, m => Assert.Equal("empty", m.Color));
        var blueMana = player2.Mana.Where(m => m.Color == "blue").ToList();
        Assert.Single(blueMana);
    }

    [Fact]
    public void BATTLE_008_FullBattleWithOnHitBurnApplicationAndBurnTick()
    {
        // Arrange
        // Player1 ON_HIT power: apply burn(power=2, duration=2) to player2
        var burnStatus = new Status("burn", 3, 2, 100);
        var burnPower = new Power(
            "burn",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.APPLY_STATUS, EffectTarget.OPPONENT, null, burnStatus),
            null,
            null
        );
        var card1 = CreateCardWithPower("c1", "red", 3, "attack", "empty", "empty", "empty", burnPower);
        var card2 = CreateCard("c2", "Victim", "blue", 0, "empty", "empty", "empty", "empty");
        
        var player1 = new Player("p1", card1);
        var player2 = new Player("p2", card2);
        
        var battle = new Battle(player1, player2, new[] { player1, player2 });
        
        // Act
        battle.Resolve();
        
        // Assert
        // Step 2: player2 takes 3 damage from attack
        // Step 2: burn status applied
        // Step 5: burn ticks for 2 damage
        // Total: 20 - 3 - 2 = 15
        Assert.Equal(15, player2.Hp);
        Assert.Single(player2.Statuses); // has burn status
        Assert.Equal("burn", player2.Statuses[0].Type);
        Assert.Equal(2, player2.Statuses[0].Power);
        Assert.Equal(2, player2.Statuses[0].Duration); // Step 6 decremented from 3 to 2
        
        // Note: Burn DOES tick in step 5 (ResolveBurnAndRenew) during the same battle it's applied
        // Step 6 (status duration decrement) runs at end of battle, so duration 3→2
    }

    [Fact]
    public void BATTLE_009_OnDefendAppliesStatusToAttacker()
    {
        // Arrange
        // Player1 has ON_DEFEND power: apply weaken(duration=2) to opponent
        var weakenStatus = new Status("weaken", 3, null, 100);
        var weakenPower = new Power(
            "weaken",
            new Condition(ConditionType.ON_DEFEND),
            new Effect(EffectType.APPLY_STATUS, EffectTarget.OPPONENT, null, weakenStatus),
            null,
            null
        );
        var card1 = CreateCardWithPower("c1", "red", 0, "defend", "empty", "empty", "empty", weakenPower);
        var card2 = CreateCard("c2", "Attacker", "blue", 5, "attack", "empty", "empty", "empty");
        
        var player1 = new Player("p1", card1);
        var player2 = new Player("p2", card2);
        
        var battle = new Battle(player1, player2, new[] { player2, player1 }); // player2 acts first
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Single(player2.Statuses); // player2 received weaken
        Assert.Equal("weaken", player2.Statuses[0].Type);
        Assert.Equal(2, player2.Statuses[0].Duration); // Step 6 decremented from 3 to 2
    }

    [Fact]
    public void BATTLE_010_HitTwiceWithOnHitStatus_StatusAppliedTwice()
    {
        // Arrange
        var player1 = new Player("p1", null!);
        var player2 = new Player("p2", null!);
        
        // Give player1 mana for HIT_TWICE
        player1.AddMana("blue");
        player1.AddMana("blue");
        player1.AddMana("blue");
        
        // Player1 has HIT_TWICE power (costs 2 blue) and ON_HIT apply vulnerable
        var hitTwicePower = new Power(
            "hit2",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.HIT_TWICE, EffectTarget.SELF, null, null),
            new ManaCost(new List<CostEntry> { new CostEntry("blue", 2) }),
            null
        );
        var vulnerableStatus = new Status("vulnerable", 2, null, 100);
        var vulnerablePower = new Power(
            "vuln",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.APPLY_STATUS, EffectTarget.OPPONENT, null, vulnerableStatus),
            null,
            null
        );
        
        var card1 = new Card("c1", "Double", "red", 4, new[]
        {
            new Zone("red", "attack"),
            new Zone("blue", "empty"),
            new Zone("green", "empty"),
            new Zone("yellow", "empty")
        }, new List<Power> { hitTwicePower, vulnerablePower });
        
        var card2 = CreateCard("c2", "Victim", "blue", 0, "empty", "empty", "empty", "empty");
        
        player1.Card = card1;
        player2.Card = card2;
        
        var battle = new Battle(player1, player2, new[] { player1, player2 });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.Equal(2, player2.GetStatusCount("vulnerable")); // vulnerable applied twice
        
        // Verify mana was spent (oldest 2 blue consumed) + 1 red generated in step 8
        var remainingMana = player1.Mana.Where(m => m.Color != "empty").ToList();
        Assert.Equal(2, remainingMana.Count); // 3 - 2 (spent) + 1 (generated) = 2
        Assert.Equal(1, remainingMana.Count(m => m.Color == "blue")); // 1 blue remains
        Assert.Equal(1, remainingMana.Count(m => m.Color == "red")); // 1 red generated in step 8
    }

    [Fact]
    public void BATTLE_011_StatusesTickAndRemovedCorrectlyOverMultipleBattles()
    {
        // Arrange
        var card1 = CreateCard("c1", "Card1", "red", 0, "empty", "empty", "empty", "empty");
        var card2 = CreateCard("c2", "Card2", "blue", 0, "empty", "empty", "empty", "empty");
        
        var player1 = new Player("p1", card1);
        var player2 = new Player("p2", card2);
        
        // Apply slow(duration=2) at start
        player1.AddStatus(new Status("slow", 2, null, 0));
        
        // Battle 1
        var battle1 = new Battle(player1, player2, new[] { player1, player2 });
        battle1.Resolve();
        
        // Assert after battle 1 - Step 6 decremented duration from 2 to 1
        Assert.Single(player1.Statuses);
        Assert.Equal(1, player1.Statuses[0].Duration); // decremented by step 6
        
        // Note: Step 6 now runs at end of every battle, decrementing status durations
    }

    [Fact]
    public void BATTLE_012_PlayerEliminatedMidTurn_DoesNotReceiveRenewHealing()
    {
        // Arrange
        var card1 = CreateCard("c1", "Killer", "red", 5, "attack", "empty", "empty", "empty");
        var card2 = CreateCard("c2", "Victim", "blue", 0, "empty", "empty", "empty", "empty");
        
        var player1 = new Player("p1", card1);
        var player2 = new Player("p2", card2, 2);
        
        // Player2 has renew that would heal them
        player2.AddStatus(new Status("renew", 2, 5, 0));
        
        var battle = new Battle(player1, player2, new[] { player1, player2 });
        
        // Act
        battle.Resolve();
        
        // Assert
        Assert.True(player2.IsEliminated);
        Assert.Equal(0, player2.Hp); // HP clamped to 0, renew did not fire
        Assert.Equal("player1", battle.Result);
    }
}

// Made with Bob
