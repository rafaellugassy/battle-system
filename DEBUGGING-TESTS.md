# How to Add Print Statements to Tests

## Quick Answer: Use Console.WriteLine or ITestOutputHelper

### Method 1: Console.WriteLine (Simple)

```csharp
[Fact]
public void MULTI_001_StatusDurationDecrementsAcrossThreeBattles()
{
    var player1 = new Player("p1", card1);
    
    // Add print statements anywhere
    Console.WriteLine($"Initial HP: {player1.Hp}");
    Console.WriteLine($"Initial status count: {player1.Statuses.Count}");
    
    player1.AddStatus(new Status("slow", 3, null, 0));
    Console.WriteLine($"After adding status: {player1.Statuses[0].Duration}");
    
    var battle1 = new Battle(player1, player2, new[] { player1, player2 });
    battle1.Resolve();
    
    Console.WriteLine($"After battle 1: Duration = {player1.Statuses[0].Duration}");
    
    battle1.DecrementStatusDurations();
    Console.WriteLine($"After decrement: Duration = {player1.Statuses[0].Duration}");
    
    Assert.Equal(2, player1.Statuses[0].Duration);
}
```

### Method 2: ITestOutputHelper (Recommended for xUnit)

```csharp
public class MultiTurnBattleTests
{
    private readonly ITestOutputHelper _output;
    
    // Add constructor
    public MultiTurnBattleTests(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    public void MULTI_001_StatusDurationDecrementsAcrossThreeBattles()
    {
        var player1 = new Player("p1", card1);
        
        _output.WriteLine($"Initial HP: {player1.Hp}");
        _output.WriteLine($"Initial status count: {player1.Statuses.Count}");
        
        player1.AddStatus(new Status("slow", 3, null, 0));
        _output.WriteLine($"After adding status: {player1.Statuses[0].Duration}");
        
        var battle1 = new Battle(player1, player2, new[] { player1, player2 });
        battle1.Resolve();
        
        _output.WriteLine($"After battle 1: Duration = {player1.Statuses[0].Duration}");
        
        Assert.Equal(2, player1.Statuses[0].Duration);
    }
}
```

## Running Tests with Output

### See Console Output

```bash
cd src
dotnet test --filter "FullyQualifiedName~MULTI_001" --logger "console;verbosity=detailed"
```

### Alternative: Run with normal verbosity

```bash
cd src
dotnet test --filter "FullyQualifiedName~MULTI_001" -v n
```

## Complete Example: Debug a Multi-Turn Test

### 1. Edit the Test File

```bash
code src/Tests/MultiTurnBattleTests.cs
```

### 2. Add Debug Output

```csharp
[Fact]
public void MULTI_002_ManaGeneratedAcrossFiveBattles()
{
    var card1 = CreateCard("c1", "Red Card", "red", 1, "attack", "empty", "empty", "empty");
    var player1 = new Player("p1", card1);
    var player2 = new Player("p2", card2);
    
    Console.WriteLine("=== Starting Test ===");
    Console.WriteLine($"Player1 initial mana count: {player1.Mana.Count(m => m.Color != "empty")}");
    
    // Battle 1
    var battle1 = new Battle(player1, player2, new[] { player1, player2 });
    battle1.Resolve();
    Console.WriteLine($"After battle 1 (before mana gen): {player1.Mana.Count(m => m.Color == "red")} red mana");
    
    battle1.GenerateMana();
    Console.WriteLine($"After battle 1 (after mana gen): {player1.Mana.Count(m => m.Color == "red")} red mana");
    
    var redMana1 = player1.Mana.Where(m => m.Color == "red").ToList();
    Assert.Single(redMana1);
    Console.WriteLine($"✓ Battle 1 passed: {redMana1.Count} red mana");
    
    // Battle 2
    var battle2 = new Battle(player1, player2, new[] { player1, player2 });
    battle2.Resolve();
    battle2.GenerateMana();
    
    var redMana2 = player1.Mana.Where(m => m.Color == "red").ToList();
    Console.WriteLine($"After battle 2: {redMana2.Count} red mana");
    Assert.Equal(2, redMana2.Count);
    Console.WriteLine($"✓ Battle 2 passed: {redMana2.Count} red mana");
    
    Console.WriteLine("=== Test Complete ===");
}
```

### 3. Run with Output

```bash
cd src
dotnet build
dotnet test --filter "FullyQualifiedName~MULTI_002" --logger "console;verbosity=detailed"
```

## Debugging Complex Objects

### Print Player State

```csharp
void PrintPlayerState(Player player, string label)
{
    Console.WriteLine($"\n=== {label} ===");
    Console.WriteLine($"HP: {player.Hp}");
    Console.WriteLine($"Eliminated: {player.IsEliminated}");
    Console.WriteLine($"Card: {player.Card?.Name ?? "null"}");
    Console.WriteLine($"Statuses: {player.Statuses.Count}");
    foreach (var status in player.Statuses)
    {
        Console.WriteLine($"  - {status.Type}: duration={status.Duration}, power={status.Power}");
    }
    Console.WriteLine($"Mana: {player.Mana.Count(m => m.Color != "empty")} filled");
    foreach (var mana in player.Mana.Where(m => m.Color != "empty"))
    {
        Console.WriteLine($"  - {mana.Color} (turn {mana.TurnGenerated})");
    }
}

[Fact]
public void DebugTest()
{
    var player1 = new Player("p1", card1);
    PrintPlayerState(player1, "Initial State");
    
    player1.AddStatus(new Status("burn", 2, 3, 0));
    PrintPlayerState(player1, "After Adding Burn");
    
    var battle = new Battle(player1, player2, turnOrder);
    battle.Resolve();
    PrintPlayerState(player1, "After Battle");
    
    battle.DecrementStatusDurations();
    PrintPlayerState(player1, "After Status Decrement");
}
```

### Print Battle State

```csharp
void PrintBattleState(Battle battle, string label)
{
    Console.WriteLine($"\n=== {label} ===");
    Console.WriteLine($"Result: {battle.Result ?? "null"}");
    Console.WriteLine($"Player1 HP: {battle.Player1.Hp}");
    Console.WriteLine($"Player2 HP: {battle.Player2.Hp}");
}

[Fact]
public void DebugBattleTest()
{
    var battle = new Battle(player1, player2, turnOrder);
    PrintBattleState(battle, "Before Battle");
    
    battle.Resolve();
    PrintBattleState(battle, "After Battle");
}
```

## Print Mana State

```csharp
void PrintManaState(Player player)
{
    Console.WriteLine($"\nMana for {player.Id}:");
    for (int i = 0; i < player.Mana.Count; i++)
    {
        var m = player.Mana[i];
        if (m.Color != "empty")
        {
            Console.WriteLine($"  [{i}] {m.Color} (turn {m.TurnGenerated})");
        }
        else
        {
            Console.WriteLine($"  [{i}] empty");
        }
    }
}

[Fact]
public void DebugManaTest()
{
    var player = new Player("p1", card);
    PrintManaState(player);
    
    player.AddMana("red");
    player.AddMana("blue");
    PrintManaState(player);
    
    player.SpendMana(new ManaCost(new List<CostEntry> { new CostEntry("red", 1) }));
    PrintManaState(player);
}
```

## Using Debugger Breakpoints (VS Code)

### 1. Install C# Dev Kit Extension

### 2. Set Breakpoint
Click in the left margin of the test file to add a red dot

### 3. Debug Test
- Open Testing panel (beaker icon)
- Right-click test → "Debug Test"
- Execution will pause at breakpoint
- Inspect variables in Debug panel

## Common Debug Patterns

### Assert with Message

```csharp
Assert.Equal(expected, actual, $"Expected {expected} but got {actual}");
```

### Print Before Assert

```csharp
Console.WriteLine($"Expected: {expected}, Actual: {actual}");
Assert.Equal(expected, actual);
```

### Print Collection Contents

```csharp
Console.WriteLine($"Statuses: [{string.Join(", ", player.Statuses.Select(s => s.Type))}]");
```

### Print with Formatting

```csharp
Console.WriteLine($"Player HP: {player.Hp,3} | Statuses: {player.Statuses.Count,2} | Mana: {player.Mana.Count(m => m.Color != "empty"),2}");
```

## Example: Full Debug Session

```csharp
[Fact]
public void MULTI_003_BurnTicksAndDurationDecrementsAcrossTwoBattles()
{
    Console.WriteLine("\n========================================");
    Console.WriteLine("TEST: Burn Ticks and Duration Decrements");
    Console.WriteLine("========================================\n");
    
    var card1 = CreateCard("c1", "Card1", "red", 0, "empty", "empty", "empty", "empty");
    var card2 = CreateCard("c2", "Card2", "blue", 0, "empty", "empty", "empty", "empty");
    
    var player1 = new Player("p1", card1);
    var player2 = new Player("p2", card2);
    
    Console.WriteLine($"Initial state:");
    Console.WriteLine($"  Player2 HP: {player2.Hp}");
    Console.WriteLine($"  Player2 Statuses: {player2.Statuses.Count}");
    
    player2.AddStatus(new Status("burn", 2, 3, 0));
    Console.WriteLine($"\nAfter adding burn(power=3, duration=2):");
    Console.WriteLine($"  Player2 HP: {player2.Hp}");
    Console.WriteLine($"  Player2 Statuses: {player2.Statuses.Count}");
    Console.WriteLine($"  Burn duration: {player2.Statuses[0].Duration}");
    Console.WriteLine($"  Burn power: {player2.Statuses[0].Power}");
    
    // Battle 1
    Console.WriteLine("\n--- Battle 1 ---");
    var battle1 = new Battle(player1, player2, new[] { player1, player2 });
    battle1.Resolve();
    
    Console.WriteLine($"After battle 1 resolve:");
    Console.WriteLine($"  Player2 HP: {player2.Hp} (expected: 17 = 20 - 3)");
    Console.WriteLine($"  Burn duration: {player2.Statuses[0].Duration} (expected: 2, unchanged)");
    
    Assert.Equal(17, player2.Hp);
    Assert.Single(player2.Statuses);
    Assert.Equal(2, player2.Statuses[0].Duration);
    Console.WriteLine("  ✓ Battle 1 assertions passed");
    
    battle1.DecrementStatusDurations();
    Console.WriteLine($"\nAfter status decrement:");
    Console.WriteLine($"  Burn duration: {player2.Statuses[0].Duration} (expected: 1)");
    Assert.Equal(1, player2.Statuses[0].Duration);
    Console.WriteLine("  ✓ Duration decrement passed");
    
    // Battle 2
    Console.WriteLine("\n--- Battle 2 ---");
    var battle2 = new Battle(player1, player2, new[] { player1, player2 });
    battle2.Resolve();
    
    Console.WriteLine($"After battle 2 resolve:");
    Console.WriteLine($"  Player2 HP: {player2.Hp} (expected: 14 = 17 - 3)");
    Console.WriteLine($"  Burn duration: {player2.Statuses[0].Duration} (expected: 1, unchanged)");
    
    Assert.Equal(14, player2.Hp);
    Assert.Single(player2.Statuses);
    Assert.Equal(1, player2.Statuses[0].Duration);
    Console.WriteLine("  ✓ Battle 2 assertions passed");
    
    battle2.DecrementStatusDurations();
    Console.WriteLine($"\nAfter status decrement:");
    Console.WriteLine($"  Player2 Statuses: {player2.Statuses.Count} (expected: 0, removed)");
    Assert.Empty(player2.Statuses);
    Console.WriteLine("  ✓ Status removal passed");
    
    Console.WriteLine("\n========================================");
    Console.WriteLine("TEST PASSED");
    Console.WriteLine("========================================\n");
}
```

### Run It

```bash
cd src
dotnet test --filter "FullyQualifiedName~MULTI_003" --logger "console;verbosity=detailed"
```

## Quick Reference

```bash
# Simple print
Console.WriteLine("Debug message");

# Print variable
Console.WriteLine($"Value: {variable}");

# Print with label
Console.WriteLine($"Player HP: {player.Hp}");

# Print collection
Console.WriteLine($"Count: {list.Count}");
Console.WriteLine($"Items: {string.Join(", ", list)}");

# Run test with output
cd src && dotnet test --filter "FullyQualifiedName~TestName" --logger "console;verbosity=detailed"
```

That's it! Add `Console.WriteLine()` anywhere in your test and run with verbose logging to see the output.