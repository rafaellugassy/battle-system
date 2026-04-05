#!/bin/bash

# Script to run a specific test and capture ALL output

echo "=========================================="
echo "Running MULTI_001 test with full output"
echo "=========================================="
echo ""

cd src

# Build first
echo "Building..."
dotnet build -v q

echo ""
echo "Running test..."
echo ""

# Run test and capture everything
dotnet test --filter "FullyQualifiedName~MULTI_001" --logger "console;verbosity=detailed" 2>&1 | tee ../test-output.log

echo ""
echo "=========================================="
echo "Test complete. Output saved to test-output.log"
echo "=========================================="
echo ""
echo "Exit code: $?"
echo ""

# Try to find and display the console output
if [ -f ../test-output.log ]; then
    echo "Searching for 'testing 3 battles' in output..."
    grep -i "testing" ../test-output.log || echo "String not found in output"
fi

# Made with Bob
