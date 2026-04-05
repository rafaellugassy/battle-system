#!/bin/bash

# Initial Push to GitHub Script
# This will push your code to GitHub for the first time

echo "╔════════════════════════════════════════╗"
echo "║   Push to GitHub                       ║"
echo "╚════════════════════════════════════════╝"
echo ""

echo "Repository: https://github.com/rafaellugassy/battle-system.git"
echo ""
echo "⚠️  IMPORTANT: GitHub no longer accepts passwords for git operations."
echo "You need to use a Personal Access Token (PAT) instead."
echo ""
echo "If you don't have a PAT yet:"
echo "1. Go to: https://github.com/settings/tokens"
echo "2. Click 'Generate new token (classic)'"
echo "3. Select scope: 'repo' (full control of private repositories)"
echo "4. Copy the token (you won't see it again!)"
echo ""
echo "When prompted for password, paste your PAT instead."
echo ""
read -p "Press Enter to continue with push, or Ctrl+C to cancel..."
echo ""

# Attempt to push
echo "Pushing to GitHub..."
git push -u origin main

if [ $? -eq 0 ]; then
    echo ""
    echo "✅ Successfully pushed to GitHub!"
    echo ""
    echo "View your repository at:"
    echo "https://github.com/rafaellugassy/battle-system"
    echo ""
    echo "Your credentials are now saved in macOS Keychain."
    echo "Future pushes won't require authentication."
else
    echo ""
    echo "❌ Push failed. Common issues:"
    echo ""
    echo "1. Authentication failed:"
    echo "   - Make sure you're using a Personal Access Token, not your password"
    echo "   - Token must have 'repo' scope"
    echo ""
    echo "2. Repository doesn't exist:"
    echo "   - Verify the repository exists at: https://github.com/rafaellugassy/battle-system"
    echo ""
    echo "3. Network issues:"
    echo "   - Check your internet connection"
    echo ""
    echo "Try again with: ./push-to-github.sh"
fi

# Made with Bob
