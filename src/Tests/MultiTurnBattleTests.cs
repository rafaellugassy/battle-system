using Xunit;
using Xunit.Abstractions;
using BattleSystem.Models;

namespace BattleSystem.Tests;

/// <summary>
/// Tests for multi-turn battle scenarios where status durations decrement
/// and mana is generated between battles (game loop simulation)
/// </summary>
public class MultiTurnBattleTests
{
    private readonly ITestOutputHelper _output;
    
    public MultiTurnBattleTests(ITestOutputHelper output)
    {
        _output = output;
    }
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
        
        var dummyPower = new Power(
            "dummy",
            new Condition(ConditionType.AFTER_TURN_RESOLVES),
            new Effect(EffectType.HEAL, EffectTarget.SELF, 0, null),
            null,
            null
        );
        
        return new Card(id, name, color, strength, zones, new List<Power> { dummyPower });
    }

    [Fact]
    public void MULTI_001_StatusDurationDecrementsAcrossThreeBattles()
    {
        _output.WriteLine("========================================");
        _output.WriteLine("TESTING: 3 battles with status duration");
        _output.WriteLine("========================================");
        
        // Arrange
        var card1 = CreateCard("c1", "Card1", "red", 2, "attack", "empty", "empty", "empty");
        var card2 = CreateCard("c2", "Card2", "blue", 1, "empty", "empty", "empty", "empty");
        
        var player1 = new Player("p1", card1);
        var player2 = new Player("p2", card2);
        
        _output.WriteLine($"Initial state: Player1 HP={player1.Hp}, Statuses={player1.Statuses.Count}");
        
        // Apply slow(duration=3) at start
        player1.AddStatus(new Status("slow", 3, null, 0));
        _output.WriteLine($"After adding slow: Duration={player1.Statuses[0].Duration}");
        
        // Battle 1
        _output.WriteLine("\n--- Battle 1 ---");
        var battle1 = new Battle(player1, player2, new[] { player1, player2 });
        battle1.Resolve();
        
        _output.WriteLine($"After battle 1 resolve: Duration={player1.Statuses[0].Duration} (expected: 2, decremented in step 6)");
        
        // After battle 1, duration should be 2 (step 6 decremented it)
        Assert.Single(player1.Statuses);
        Assert.Equal("slow", player1.Statuses[0].Type);
        Assert.Equal(2, player1.Statuses[0].Duration);
        _output.WriteLine("✓ Battle 1 passed");
        
        // Battle 2
        _output.WriteLine("\n--- Battle 2 ---");
        var battle2 = new Battle(player1, player2, new[] { player1, player2 });
        battle2.Resolve();
        
        _output.WriteLine($"After battle 2: Duration={player1.Statuses[0].Duration} (expected: 1)");
        
        // After battle 2, duration should be 1
        Assert.Single(player1.Statuses);
        Assert.Equal(1, player1.Statuses[0].Duration);
        _output.WriteLine("✓ Battle 2 passed");
        
        // Battle 3
        _output.WriteLine("\n--- Battle 3 ---");
        var battle3 = new Battle(player1, player2, new[] { player1, player2 });
        battle3.Resolve();
        
        _output.WriteLine($"After battle 3: Statuses count={player1.Statuses.Count} (expected: 0, removed)");
        
        // After battle 3, status should be removed (duration reached 0)
        Assert.Empty(player1.Statuses);
        _output.WriteLine("✓ Battle 3 passed - Status removed!");
        _output.WriteLine("\n========================================");
        _output.WriteLine("TEST COMPLETE - ALL PASSED");
        _output.WriteLine("========================================");
    }

    [Fact]
    public void MULTI_002_ManaGeneratedAcrossFiveBattles()
    {
        // Arrange
        var card1 = CreateCard("c1", "Red Card", "red", 1, "attack", "empty", "empty", "empty");
        var card2 = CreateCard("c2", "Blue Card", "blue", 0, "empty", "empty", "empty", "empty");
        
        var player1 = new Player("p1", card1);
        var player2 = new Player("p2", card2);
        
        // Verify starting state - all empty
        Assert.All(player1.Mana, m => Assert.Equal("empty", m.Color));
        
        // Battle 1
        var battle1 = new Battle(player1, player2, new[] { player1, player2 });
        battle1.Resolve();
        
        // After battle 1, mana generated in step 8
        // Player1 should have 1 red mana
        var redMana1 = player1.Mana.Where(m => m.Color == "red").ToList();
        Assert.Single(redMana1);
        Assert.Equal(1, redMana1[0].TurnGenerated); // generated after battle 1
        
        // Battle 2
        var battle2 = new Battle(player1, player2, new[] { player1, player2 });
        battle2.Resolve();
        
        // After battle 2, player1 should have 2 red mana
        var redMana2 = player1.Mana.Where(m => m.Color == "red").ToList();
        Assert.Equal(2, redMana2.Count);
        
        // Battle 3
        var battle3 = new Battle(player1, player2, new[] { player1, player2 });
        battle3.Resolve();
        
        // After battle 3, player1 should have 3 red mana
        var redMana3 = player1.Mana.Where(m => m.Color == "red").ToList();
        Assert.Equal(3, redMana3.Count);
        
        // Battle 4
        var battle4 = new Battle(player1, player2, new[] { player1, player2 });
        battle4.Resolve();
        
        // After battle 4, player1 should have 4 red mana
        var redMana4 = player1.Mana.Where(m => m.Color == "red").ToList();
        Assert.Equal(4, redMana4.Count);
        
        // Battle 5
        var battle5 = new Battle(player1, player2, new[] { player1, player2 });
        battle5.Resolve();
        
        // After battle 5, player1 should have 5 red mana (max capacity)
        var redMana5 = player1.Mana.Where(m => m.Color == "red").ToList();
        Assert.Equal(5, redMana5.Count);
        
        // Verify turn numbers are correct (1, 2, 3, 4, 5)
        Assert.Equal(1, redMana5[0].TurnGenerated);
        Assert.Equal(2, redMana5[1].TurnGenerated);
        Assert.Equal(3, redMana5[2].TurnGenerated);
        Assert.Equal(4, redMana5[3].TurnGenerated);
        Assert.Equal(5, redMana5[4].TurnGenerated);
    }

    [Fact]
    public void MULTI_003_BurnTicksAndDurationDecrementsAcrossTwoBattles()
    {
        // Arrange
        var card1 = CreateCard("c1", "Card1", "red", 0, "empty", "empty", "empty", "empty");
        var card2 = CreateCard("c2", "Card2", "blue", 0, "empty", "empty", "empty", "empty");
        
        var player1 = new Player("p1", card1);
        var player2 = new Player("p2", card2);
        
        // Apply burn(power=3, duration=2) at start
        player2.AddStatus(new Status("burn", 2, 3, 0));
        
        // Battle 1
        var battle1 = new Battle(player1, player2, new[] { player1, player2 });
        battle1.Resolve();
        
        // After battle 1, burn ticked in step 5, duration decremented in step 6
        Assert.Equal(17, player2.Hp); // 20 - 3 = 17
        Assert.Single(player2.Statuses);
        Assert.Equal(1, player2.Statuses[0].Duration); // duration decremented from 2 to 1
        
        // Battle 2
        var battle2 = new Battle(player1, player2, new[] { player1, player2 });
        battle2.Resolve();
        
        // After battle 2, burn ticked again, status removed (duration reached 0)
        Assert.Equal(14, player2.Hp); // 17 - 3 = 14
        Assert.Empty(player2.Statuses); // status removed after duration reached 0
    }

    [Fact]
    public void MULTI_004_MultipleStatusesTickAndExpireIndependently()
    {
        // Arrange
        var card1 = CreateCard("c1", "Card1", "red", 0, "empty", "empty", "empty", "empty");
        var card2 = CreateCard("c2", "Card2", "blue", 0, "empty", "empty", "empty", "empty");
        
        var player1 = new Player("p1", card1);
        var player2 = new Player("p2", card2);
        
        // Apply multiple statuses with different durations
        player1.AddStatus(new Status("slow", 1, null, 0));      // expires after 1 battle
        player1.AddStatus(new Status("weaken", 3, null, 0));    // expires after 3 battles
        player1.AddStatus(new Status("vulnerable", 2, null, 0)); // expires after 2 battles
        
        // Battle 1
        var battle1 = new Battle(player1, player2, new[] { player1, player2 });
        battle1.Resolve();
        
        // After battle 1: slow expires (duration 1→0), others remain
        Assert.Equal(2, player1.Statuses.Count);
        Assert.Contains(player1.Statuses, s => s.Type == "weaken" && s.Duration == 2);
        Assert.Contains(player1.Statuses, s => s.Type == "vulnerable" && s.Duration == 1);
        
        // Battle 2
        var battle2 = new Battle(player1, player2, new[] { player1, player2 });
        battle2.Resolve();
        
        // After battle 2: vulnerable expires (duration 1→0), weaken remains
        Assert.Single(player1.Statuses);
        Assert.Equal("weaken", player1.Statuses[0].Type);
        Assert.Equal(1, player1.Statuses[0].Duration);
        
        // Battle 3
        var battle3 = new Battle(player1, player2, new[] { player1, player2 });
        battle3.Resolve();
        
        // After battle 3: all statuses expired
        Assert.Empty(player1.Statuses);
    }

    [Fact]
    public void MULTI_005_ManaSpentAndRegeneratedAcrossBattles()
    {
        // Arrange
        var player1 = new Player("p1", null!);
        var player2 = new Player("p2", null!);
        
        // Give player1 starting mana
        player1.AddMana("blue");
        player1.AddMana("blue");
        
        // Player1 has HIT_TWICE power (costs 2 blue)
        var hitTwicePower = new Power(
            "hit2",
            new Condition(ConditionType.ON_HIT),
            new Effect(EffectType.HIT_TWICE, EffectTarget.SELF, null, null),
            new ManaCost(new List<CostEntry> { new CostEntry("blue", 2) }),
            null
        );
        
        var card1 = new Card("c1", "Blue Card", "blue", 4, new[]
        {
            new Zone("red", "attack"),
            new Zone("blue", "empty"),
            new Zone("green", "empty"),
            new Zone("yellow", "empty")
        }, new List<Power> { hitTwicePower });
        
        var card2 = CreateCard("c2", "Victim", "red", 0, "empty", "empty", "empty", "empty");
        
        player1.Card = card1;
        player2.Card = card2;
        
        // Battle 1 - spend 2 blue mana
        var battle1 = new Battle(player1, player2, new[] { player1, player2 });
        battle1.Resolve();
        
        // After battle 1, mana spent + 1 blue generated in step 8
        var blueMana1 = player1.Mana.Where(m => m.Color == "blue").ToList();
        Assert.Single(blueMana1);
        
        // Battle 2 - not enough mana to use power
        var battle2 = new Battle(player1, player2, new[] { player1, player2 });
        battle2.Resolve();
        
        // After battle 2, 2 blue mana total
        var blueMana2 = player1.Mana.Where(m => m.Color == "blue").ToList();
        Assert.Equal(2, blueMana2.Count);
        
        // Battle 3 - can use power again
        var battle3 = new Battle(player1, player2, new[] { player1, player2 });
        battle3.Resolve();
        
        // After battle 3, mana spent (2 blue) + 1 blue generated in step 8
        var blueMana3 = player1.Mana.Where(m => m.Color == "blue").ToList();
        Assert.Single(blueMana3);
    }

    [Fact]
    public void MULTI_006_CompleteGameLoopSimulation()
    {
        // Arrange - simulate a complete 3-turn game
        var card1 = CreateCard("c1", "Red Attacker", "red", 3, "attack", "empty", "empty", "empty");
        var card2 = CreateCard("c2", "Blue Defender", "blue", 2, "defend", "empty", "empty", "empty");
        
        var player1 = new Player("p1", card1);
        var player2 = new Player("p2", card2);
        
        // Apply initial status - protect halves damage taken
        player1.AddStatus(new Status("protect", 2, null, 0));
        
        // Turn 1
        var battle1 = new Battle(player1, player2, new[] { player1, player2 });
        battle1.Resolve();
        
        // Verify battle 1 results - red attack blocked by blue defend
        Assert.Equal(20, player1.Hp);
        Assert.Equal(20, player2.Hp); // attack blocked, no damage
        Assert.Single(player1.Statuses);
        // Status duration decremented from 2 to 1 in step 6
        Assert.Equal(1, player1.Statuses[0].Duration);
        // Mana generated in step 8
        Assert.Single(player1.Mana.Where(m => m.Color == "red"));
        
        // Turn 2
        var battle2 = new Battle(player1, player2, new[] { player1, player2 });
        battle2.Resolve();
        
        // Verify battle 2 results - attack still blocked
        Assert.Equal(20, player1.Hp);
        Assert.Equal(20, player2.Hp); // attack blocked, no damage
        
        // Mana generated automatically in step 8
        
        // Verify status expired (duration reached 0 in step 6)
        Assert.Empty(player1.Statuses);
        Assert.Equal(2, player1.Mana.Where(m => m.Color == "red").Count());
        
        // Turn 3
        var battle3 = new Battle(player1, player2, new[] { player1, player2 });
        battle3.Resolve();
        
        // Verify battle 3 results - attack still blocked
        Assert.Equal(20, player1.Hp);
        Assert.Equal(20, player2.Hp); // attack blocked, no damage
        
        // Mana generated automatically in step 8
        
        // Verify final state
        Assert.Equal(3, player1.Mana.Where(m => m.Color == "red").Count());
    }
}

// Made with Bob