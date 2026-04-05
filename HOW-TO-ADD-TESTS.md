# How to Add or Modify Tests

## Quick Workflow: Make a Change and Test It

### 1. Edit Code
Make your change in any file under `src/Models/` or `src/Tests/`

### 2. Build and Test
```bash
cd src
dotnet build
dotnet test
```

That's it! If exit code is 0, all tests pass.

## Example: Adding a New Test

### Step 1: Open or Create Test File

For example, to add a test to `src/Tests/MultiTurnBattleTests.cs`:

```bash
# Open in your editor
code src/Tests/MultiTurnBattleTests.cs
```

### Step 2: Add Your Test Method

```csharp
[Fact]
public void MULTI_007_YourNewTestName()
{
    // Arrange - Set up test data
    var card1 = CreateCard("c1", "Card1", "red", 5, "attack", "empty", "empty", "empty");
    var player1 = new Player("p1", card1);
    var player2 = new Player("p2", card1);
    
    // Act - Execute the code being tested
    var battle = new Battle(player1, player2, new[] { player1, player2 });
    battle.Resolve();
    
    // Assert - Verify expected results
    Assert.Equal(15, player2.Hp); // Expected: 20 - 5 = 15
}
```

### Step 3: Build and Run

```bash
cd src
dotnet build
dotnet test --filter "FullyQualifiedName~MULTI_007"
```

### Step 4: Verify It Passes

```bash
echo $?  # Should be 0
```

## Example: Modifying Existing Code

### Scenario: Change Battle Logic

1. **Edit the code:**
```bash
code src/Models/Battle.cs
```

2. **Make your change** (e.g., modify damage calculation)

3. **Run affected tests:**
```bash
cd src
dotnet test --filter "FullyQualifiedName~DamageCalculation"
```

4. **If tests fail, fix code or update test expectations**

5. **Run all tests to ensure nothing broke:**
```bash
cd src
dotnet test
```

## Test-Driven Development (TDD) Workflow

### 1. Write a Failing Test First

```csharp
[Fact]
public void NewFeature_ShouldDoSomething()
{
    // Arrange
    var player = new Player("p1", null!);
    
    // Act
    player.NewMethod(); // This doesn't exist yet!
    
    // Assert
    Assert.Equal(expectedValue, player.SomeProperty);
}
```

### 2. Run Test (It Will Fail)

```bash
cd src
dotnet build  # Will fail - method doesn't exist
```

### 3. Implement the Feature

Edit `src/Models/Player.cs` and add the method:

```csharp
public void NewMethod()
{
    // Implementation
}
```

### 4. Run Test Again (Should Pass)

```bash
cd src
dotnet build
dotnet test --filter "FullyQualifiedName~NewFeature"
```

## Common Test Patterns

### Testing Status Effects

```csharp
[Fact]
public void Status_ShouldExpireAfterDuration()
{
    var player = new Player("p1", null!);
    player.AddStatus(new Status("burn", 2, 3, 0));
    
    var battle = new Battle(player, opponent, turnOrder);
    battle.Resolve();
    battle.DecrementStatusDurations();
    
    Assert.Equal(1, player.Statuses[0].Duration);
}
```

### Testing Mana Generation

```csharp
[Fact]
public void Mana_ShouldGenerateAfterBattle()
{
    var card = CreateCard("c1", "Red", "red", 5, "attack", "empty", "empty", "empty");
    var player = new Player("p1", card);
    
    var battle = new Battle(player, opponent, turnOrder);
    battle.Resolve();
    battle.GenerateMana();
    
    var redMana = player.Mana.Where(m => m.Color == "red").ToList();
    Assert.Single(redMana);
}
```

### Testing Power Resolution

```csharp
[Fact]
public void Power_ShouldTriggerOnCondition()
{
    var power = new Power(
        "test",
        new Condition(ConditionType.ON_HIT),
        new Effect(EffectType.HEAL, EffectTarget.SELF, 5, null),
        null,
        null
    );
    
    var card = CreateCardWithPower("c1", "red", 3, "attack", "empty", "empty", "empty", power);
    var player = new Player("p1", card);
    
    var battle = new Battle(player, opponent, turnOrder);
    battle.Resolve();
    
    Assert.Equal(25, player.Hp); // 20 + 5 heal
}
```

## Running Specific Test Categories

```bash
# Run all battle tests
cd src && dotnet test --filter "FullyQualifiedName~BattleTests"

# Run all multi-turn tests
cd src && dotnet test --filter "FullyQualifiedName~MultiTurnBattleTests"

# Run all status tests
cd src && dotnet test --filter "FullyQualifiedName~StatusTests"

# Run all mana tests
cd src && dotnet test --filter "FullyQualifiedName~ManaTests"

# Run all power tests
cd src && dotnet test --filter "FullyQualifiedName~PowerResolutionTests"

# Run all damage tests
cd src && dotnet test --filter "FullyQualifiedName~DamageCalculationTests"
```

## Debugging Failed Tests

### 1. Run with Verbose Output

```bash
cd src
dotnet test --filter "FullyQualifiedName~YourFailingTest" --logger "console;verbosity=detailed"
```

### 2. Add Debug Output to Test

```csharp
[Fact]
public void DebugTest()
{
    var player = new Player("p1", card);
    Console.WriteLine($"Initial HP: {player.Hp}");
    
    battle.Resolve();
    Console.WriteLine($"After battle HP: {player.Hp}");
    
    Assert.Equal(expectedHp, player.Hp);
}
```

### 3. Run Single Test

```bash
cd src
dotnet test --filter "FullyQualifiedName~DebugTest"
```

## Best Practices

1. **One assertion per test** (when possible)
2. **Clear test names** that describe what's being tested
3. **Arrange-Act-Assert** pattern
4. **Test edge cases** (zero values, null, max values)
5. **Test error conditions** (invalid input, eliminated players)
6. **Keep tests independent** (don't rely on test execution order)

## Quick Reference

```bash
# Build only
cd src && dotnet build

# Test only (no build)
cd src && dotnet test --no-build

# Clean, build, test
cd src && dotnet clean && dotnet build && dotnet test

# Test with filter
cd src && dotnet test --filter "FullyQualifiedName~TestName"

# Count tests
cd src/Tests && grep -c "^\s*\[Fact\]" *.cs

# Check exit code
cd src && dotnet test; echo $?
```

## Example: Complete Workflow

```bash
# 1. Make a change
code src/Models/Battle.cs

# 2. Build
cd src && dotnet build

# 3. Run related tests
cd src && dotnet test --filter "FullyQualifiedName~BattleTests"

# 4. If tests fail, fix and repeat
code src/Models/Battle.cs
cd src && dotnet build && dotnet test --filter "FullyQualifiedName~BattleTests"

# 5. Run all tests to ensure nothing broke
cd src && dotnet test

# 6. Verify success
echo $?  # Should be 0
```

## Creating a New Test File

```bash
# Create new test file
cat > src/Tests/MyNewTests.cs << 'EOF'
using Xunit;
using BattleSystem.Models;

namespace BattleSystem.Tests;

public class MyNewTests
{
    [Fact]
    public void MyFirstTest()
    {
        // Arrange
        var player = new Player("p1", null!);
        
        // Act
        // ... do something
        
        // Assert
        Assert.NotNull(player);
    }
}
EOF

# Build and test
cd src && dotnet build && dotnet test --filter "FullyQualifiedName~MyNewTests"
```

That's it! You're ready to add and test changes.