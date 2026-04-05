# Prerequisites Installation Guide for macOS

## Current Status
✅ **Git** - Already installed (version 2.51.0)
✅ **Homebrew** - Already installed (version 5.0.14)
✅ **Bash** - Built into macOS
❌ **.NET SDK** - Needs installation (requires password)

## Install .NET SDK

### Option 1: Using Homebrew (Recommended)
Run this command in your terminal and enter your password when prompted:

```bash
brew install --cask dotnet-sdk
```

### Option 2: Direct Download from Microsoft
1. Visit: https://dotnet.microsoft.com/download
2. Download .NET 8.0 SDK for macOS (ARM64)
3. Run the installer package
4. Follow the installation wizard

### Verify Installation
After installation, verify it worked:

```bash
dotnet --version
```

You should see something like: `10.0.201` or `8.0.xxx`

## Test Prerequisites

Run this script to verify all prerequisites are installed:

```bash
#!/bin/bash
echo "Checking prerequisites..."
echo ""

# Check Git
if command -v git &> /dev/null; then
    echo "✅ Git: $(git --version)"
else
    echo "❌ Git: Not installed"
fi

# Check .NET
if command -v dotnet &> /dev/null; then
    echo "✅ .NET SDK: $(dotnet --version)"
else
    echo "❌ .NET SDK: Not installed"
fi

# Check Bash
if [ -n "$BASH_VERSION" ]; then
    echo "✅ Bash: $BASH_VERSION"
else
    echo "❌ Bash: Not available"
fi

# Check Homebrew
if command -v brew &> /dev/null; then
    echo "✅ Homebrew: $(brew --version | head -n1)"
else
    echo "❌ Homebrew: Not installed"
fi

echo ""
echo "All prerequisites checked!"
```

## After Installing .NET SDK

Once .NET SDK is installed, you can:

1. **Initialize the C# project:**
   ```bash
   dotnet new console -n BattleSystem -o src
   cd src
   dotnet new xunit -n BattleSystem.Tests -o tests
   ```

2. **Run the Ralph Wiggum loop:**
   ```bash
   cd ..
   ./ralph-wiggum-loop.sh
   ```

## Alternative: Use Without .NET SDK

If you prefer not to install .NET SDK right now, you can:

1. **Modify the loop to skip .NET checks** - Edit `ralph-wiggum-loop.sh` and comment out the dotnet test check
2. **Use a different language** - Modify `prompt.md` to specify a different language (Python, JavaScript, etc.)
3. **Manual implementation** - Follow the specs manually without the automated loop

## Need Help?

If you encounter issues:
- Check that you have admin rights on your Mac
- Ensure you're connected to the internet
- Try restarting your terminal after installation
- Run `brew doctor` to check for Homebrew issues

## Next Steps

After .NET SDK is installed:
1. Run the verification script above
2. Initialize your C# project structure
3. Start the Ralph Wiggum loop: `./ralph-wiggum-loop.sh`