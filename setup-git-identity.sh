#!/bin/bash

# Git Identity Setup Script
# Use this to configure git user for this repository

echo "╔════════════════════════════════════════╗"
echo "║   Git Identity Setup                   ║"
echo "╚════════════════════════════════════════╝"
echo ""

# Check current global config
echo "Current global git configuration:"
GLOBAL_NAME=$(git config --global user.name 2>/dev/null)
GLOBAL_EMAIL=$(git config --global user.email 2>/dev/null)

if [ -n "$GLOBAL_NAME" ]; then
    echo "  Name:  $GLOBAL_NAME"
else
    echo "  Name:  (not set)"
fi

if [ -n "$GLOBAL_EMAIL" ]; then
    echo "  Email: $GLOBAL_EMAIL"
else
    echo "  Email: (not set)"
fi

echo ""
echo "════════════════════════════════════════"
echo ""

# Ask if user wants to set repository-specific config
echo "Do you want to set a different identity for THIS repository only?"
echo "(This is useful if pushing to a different GitHub account)"
echo ""
read -p "Set repository-specific identity? (y/n): " -n 1 -r
echo ""

if [[ $REPLY =~ ^[Yy]$ ]]; then
    echo ""
    read -p "Enter your name: " USER_NAME
    read -p "Enter your email: " USER_EMAIL
    
    git config user.name "$USER_NAME"
    git config user.email "$USER_EMAIL"
    
    echo ""
    echo "✅ Repository-specific identity set:"
    echo "  Name:  $(git config user.name)"
    echo "  Email: $(git config user.email)"
    echo ""
    echo "This will only apply to commits in this repository."
else
    if [ -z "$GLOBAL_NAME" ] || [ -z "$GLOBAL_EMAIL" ]; then
        echo ""
        echo "⚠️  No global git identity found. Setting one now..."
        echo ""
        read -p "Enter your name: " USER_NAME
        read -p "Enter your email: " USER_EMAIL
        
        git config --global user.name "$USER_NAME"
        git config --global user.email "$USER_EMAIL"
        
        echo ""
        echo "✅ Global git identity set:"
        echo "  Name:  $(git config --global user.name)"
        echo "  Email: $(git config --global user.email)"
    else
        echo ""
        echo "✅ Using global git identity for this repository"
    fi
fi

echo ""
echo "════════════════════════════════════════"
echo ""
echo "Current identity for this repository:"
echo "  Name:  $(git config user.name)"
echo "  Email: $(git config user.email)"
echo ""
echo "Ready to commit and push! 🚀"

# Made with Bob
