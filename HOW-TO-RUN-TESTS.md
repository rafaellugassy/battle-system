# How to Run Tests Manually

## Quick Commands

### Run All Tests
```bash
cd src
dotnet test
```

### Run Specific Test File
```bash
cd src
dotnet test --filter "FullyQualifiedName~MultiTurnBattleTests"
dotnet test --filter "FullyQualifiedName~BattleTests"
dotnet test --filter "FullyQualifiedName~StatusTests"
```

### Run Single Test
```bash
cd src
dotnet test --filter "FullyQualifiedName~MULTI_001"
```

### Run with Verbose Output
```bash
cd src
dotnet test --logger "console;verbosity=detailed"
```

### Build and Run Tests Separately
```bash
cd src
dotnet build
dotnet test --no-build
```

## Using VS Code Test Explorer

1. Install the **C# Dev Kit** extension in VS Code
2. Open the Testing panel (beaker icon in sidebar)
3. Click "Run All Tests" or run individual tests
4. View test results in the Test Explorer

## Using Visual Studio (if available)

1. Open `src/BattleSystem.csproj` in Visual Studio
2. Go to Test > Test Explorer
3. Click "Run All" or right-click individual tests

## Verify Test Count

```bash
cd src/Tests
grep -h "^\s*\[Fact\]" *.cs | wc -l
```

Should show: **106 tests**

Breakdown:
- ZoneTests.cs: 5 tests
- ManaTests.cs: 11 tests
- StatusTests.cs: 22 tests
- GreatestHitTests.cs: 8 tests
- DamageCalculationTests.cs: 21 tests
- PowerResolutionTests.cs: 20 tests
- BattleTests.cs: 12 tests
- MultiTurnBattleTests.cs: 6 tests
- UnitTest1.cs: 1 test (placeholder)

## Check Test Results

Exit code 0 = all tests passed
Exit code 1 = some tests failed

```bash
cd src
dotnet test
echo $?  # Shows exit code
```

## Multi-Turn Battle Tests Specifically

The 6 multi-turn tests demonstrate the game loop pattern:

```bash
cd src
dotnet test --filter "FullyQualifiedName~MULTI_001" # Status duration decrements
dotnet test --filter "FullyQualifiedName~MULTI_002" # Mana generation
dotnet test --filter "FullyQualifiedName~MULTI_003" # Burn ticks and duration
dotnet test --filter "FullyQualifiedName~MULTI_004" # Multiple statuses
dotnet test --filter "FullyQualifiedName~MULTI_005" # Mana spent/regenerated
dotnet test --filter "FullyQualifiedName~MULTI_006" # Complete game loop
```

## Troubleshooting

If tests don't run:
1. Ensure .NET 10.0 SDK is installed: `dotnet --version`
2. Restore packages: `cd src && dotnet restore`
3. Clean and rebuild: `cd src && dotnet clean && dotnet build`
4. Check for compilation errors: `cd src && dotnet build`

## Example: Running Multi-Turn Tests with Output

```bash
cd src
dotnet clean
dotnet build
dotnet test --filter "FullyQualifiedName~MultiTurnBattleTests" --logger "console;verbosity=normal"
```

If output is suppressed, check exit code:
```bash
cd src
dotnet test --filter "FullyQualifiedName~MultiTurnBattleTests"
echo "Exit code: $?"
```

Exit code 0 = all 6 multi-turn tests passed ✅