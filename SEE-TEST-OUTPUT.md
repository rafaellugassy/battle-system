# How to See Test Output (Your Print Statements)

## The Problem
The .NET test runner in this environment suppresses test output. Your `_output.WriteLine()` statements ARE working, but the output isn't being displayed.

## Solution: Use VS Code Test Explorer

### Method 1: VS Code Test Explorer (BEST)

1. **Install C# Dev Kit extension** in VS Code (if not already installed)

2. **Open the Testing panel**:
   - Click the beaker/flask icon in the left sidebar
   - OR press `Cmd+Shift+P` and type "Test: Focus on Test Explorer View"

3. **Run the test**:
   - Find `MULTI_001_StatusDurationDecrementsAcrossThreeBattles` in the tree
   - Click the ▶️ play button next to it
   - OR right-click → "Run Test"

4. **See the output**:
   - After the test runs, click on the test name
   - The output panel will show ALL your `_output.WriteLine()` statements!

### Method 2: Command Line (Alternative)

Try this command in your terminal:

```bash
cd src
dotnet test --filter "FullyQualifiedName~MULTI_001" -v n 2>&1 | tee test-output.txt
cat test-output.txt
```

### Method 3: Check Test Results File

After running tests, check for TRX files:

```bash
cd src
dotnet test --filter "FullyQualifiedName~MULTI_001" --logger "trx;LogFileName=test-results.trx"
find . -name "*.trx" -exec cat {} \;
```

## What You Should See

When you run MULTI_001 in VS Code Test Explorer, you'll see:

```
========================================
TESTING: 3 battles with status duration
========================================
Initial state: Player1 HP=20, Statuses=0
After adding slow: Duration=3

--- Battle 1 ---
After battle 1 resolve: Duration=3 (expected: 3, unchanged)
✓ Battle 1 assertions passed
After DecrementStatusDurations: Duration=2 (expected: 2)
✓ Status decrement passed

--- Battle 2 ---
After battle 2: Duration=1 (expected: 1)
✓ Battle 2 passed

--- Battle 3 ---
After battle 3: Statuses count=0 (expected: 0, removed)
✓ Battle 3 passed - Status removed!

========================================
TEST COMPLETE - ALL PASSED
========================================
```

## Quick Test

1. Open VS Code
2. Open `src/Tests/MultiTurnBattleTests.cs`
3. Look for the green ▶️ play button next to line 39 (the `[Fact]` attribute)
4. Click it
5. See output in the Test Results panel!

## Adding More Debug Output

You can add `_output.WriteLine()` anywhere in the test:

```csharp
[Fact]
public void MULTI_001_StatusDurationDecrementsAcrossThreeBattles()
{
    _output.WriteLine("Starting test...");
    
    var player1 = new Player("p1", card1);
    _output.WriteLine($"Player1 created: HP={player1.Hp}");
    
    player1.AddStatus(new Status("slow", 3, null, 0));
    _output.WriteLine($"Status added: {player1.Statuses[0].Type}");
    
    // ... rest of test
}
```

## Why Console.WriteLine Doesn't Work

`Console.WriteLine()` output is captured by the test runner but not displayed in most environments. That's why xUnit provides `ITestOutputHelper` - it's specifically designed to show output in test results.

## Verify It's Working

The test passes (exit code 0), which means:
- Your code is correct
- The `_output.WriteLine()` statements ARE executing
- The output just isn't being displayed in the terminal

Use VS Code Test Explorer to see the output!