#!/bin/bash

# Prerequisites Checker for Ralph Wiggum Loop
# Run this to verify all required tools are installed

echo "╔════════════════════════════════════════╗"
echo "║   Prerequisites Check                  ║"
echo "╚════════════════════════════════════════╝"
echo ""

ALL_GOOD=true

# Check Git
echo -n "Checking Git... "
if command -v git &> /dev/null; then
    VERSION=$(git --version)
    echo "✅ $VERSION"
else
    echo "❌ Not installed"
    ALL_GOOD=false
fi

# Check .NET SDK
echo -n "Checking .NET SDK... "
if command -v dotnet &> /dev/null; then
    VERSION=$(dotnet --version)
    echo "✅ Version $VERSION"
else
    echo "❌ Not installed"
    echo "   Install with: brew install --cask dotnet-sdk"
    ALL_GOOD=false
fi

# Check Bash
echo -n "Checking Bash... "
if [ -n "$BASH_VERSION" ]; then
    echo "✅ Version $BASH_VERSION"
else
    echo "❌ Not available"
    ALL_GOOD=false
fi

# Check Homebrew
echo -n "Checking Homebrew... "
if command -v brew &> /dev/null; then
    VERSION=$(brew --version | head -n1)
    echo "✅ $VERSION"
else
    echo "❌ Not installed"
    ALL_GOOD=false
fi

echo ""
echo "════════════════════════════════════════"

if [ "$ALL_GOOD" = true ]; then
    echo "✅ All prerequisites installed!"
    echo ""
    echo "You're ready to run the Ralph Wiggum loop:"
    echo "  ./ralph-wiggum-loop.sh"
    exit 0
else
    echo "❌ Some prerequisites are missing"
    echo ""
    echo "Please install missing items. See INSTALL-PREREQUISITES.md"
    exit 1
fi

# Made with Bob
