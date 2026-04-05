#!/bin/bash

echo "========================================"
echo "PROOF THAT TESTS ARE WORKING"
echo "========================================"
echo ""

cd src

echo "Step 1: Running tests with CORRECT assertion (should pass)..."
echo "--------------------------------------"

# First, fix the test
cat > /tmp/fix.txt << 'EOF'
        Assert.Equal(2, player1.Statuses[0].Duration);
EOF

# Replace line 75 with correct assertion
sed -i.bak '75s/.*/        Assert.Equal(2, player1.Statuses[0].Duration);/' Tests/MultiTurnBattleTests.cs

dotnet build -v q > /dev/null 2>&1
dotnet test --filter "FullyQualifiedName~MULTI_001" --no-build > /dev/null 2>&1
RESULT1=$?

if [ $RESULT1 -eq 0 ]; then
    echo "✅ TEST PASSED (exit code 0)"
else
    echo "❌ TEST FAILED (exit code $RESULT1)"
fi

echo ""
echo "Step 2: Breaking the test with WRONG assertion (should fail)..."
echo "--------------------------------------"

# Now break it
sed -i.bak '75s/.*/        Assert.Equal(999, player1.Statuses[0].Duration); \/\/ WRONG!/' Tests/MultiTurnBattleTests.cs

dotnet build -v q > /dev/null 2>&1
dotnet test --filter "FullyQualifiedName~MULTI_001" --no-build > /dev/null 2>&1
RESULT2=$?

if [ $RESULT2 -eq 0 ]; then
    echo "✅ TEST PASSED (exit code 0) - UNEXPECTED!"
else
    echo "❌ TEST FAILED (exit code $RESULT2) - EXPECTED!"
fi

echo ""
echo "Step 3: Fixing it again (should pass)..."
echo "--------------------------------------"

# Fix it again
sed -i.bak '75s/.*/        Assert.Equal(2, player1.Statuses[0].Duration);/' Tests/MultiTurnBattleTests.cs

dotnet build -v q > /dev/null 2>&1
dotnet test --filter "FullyQualifiedName~MULTI_001" --no-build > /dev/null 2>&1
RESULT3=$?

if [ $RESULT3 -eq 0 ]; then
    echo "✅ TEST PASSED (exit code 0)"
else
    echo "❌ TEST FAILED (exit code $RESULT3)"
fi

echo ""
echo "========================================"
echo "CONCLUSION:"
echo "========================================"
echo ""

if [ $RESULT1 -eq 0 ] && [ $RESULT2 -ne 0 ] && [ $RESULT3 -eq 0 ]; then
    echo "✅ TESTS ARE WORKING CORRECTLY!"
    echo ""
    echo "Evidence:"
    echo "  - Correct assertion: PASSED ✓"
    echo "  - Wrong assertion (999): FAILED ✗"
    echo "  - Fixed assertion: PASSED ✓"
    echo ""
    echo "The tests ARE running and validating your code!"
else
    echo "⚠️  Unexpected results:"
    echo "  - Test 1 (correct): exit code $RESULT1"
    echo "  - Test 2 (broken): exit code $RESULT2"
    echo "  - Test 3 (fixed): exit code $RESULT3"
fi

echo ""

# Made with Bob
