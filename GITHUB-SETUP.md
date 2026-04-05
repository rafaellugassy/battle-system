# GitHub Remote Repository Setup Guide

## Overview
This guide shows you how to create a GitHub repository (on any account) and push your Ralph Wiggum loop project to it.

## Option 1: Create New Repository on GitHub (Recommended)

### Step 1: Create Repository on GitHub
1. Go to https://github.com (log into the account you want to use)
2. Click the **"+"** icon in top right → **"New repository"**
3. Fill in details:
   - **Repository name**: `battle-system` (or your preferred name)
   - **Description**: "Battle system implementation using Ralph Wiggum AI loop"
   - **Visibility**: Choose Public or Private
   - **DO NOT** initialize with README, .gitignore, or license (we already have these)
4. Click **"Create repository"**

### Step 2: Connect Your Local Repository
After creating the repo, GitHub will show you commands. Use these:

```bash
# Add the remote repository
git remote add origin https://github.com/USERNAME/battle-system.git

# Verify the remote was added
git remote -v

# Push your code
git branch -M main
git push -u origin main
```

Replace `USERNAME` with the actual GitHub username.

### Step 3: Verify
Visit your repository URL to see your files:
```
https://github.com/USERNAME/battle-system
```

## Option 2: Using SSH (More Secure)

### Step 1: Set Up SSH Key (if you haven't already)

Check if you have an SSH key:
```bash
ls -la ~/.ssh
```

If you don't see `id_ed25519.pub` or `id_rsa.pub`, create one:
```bash
# Generate new SSH key
ssh-keygen -t ed25519 -C "your_email@example.com"

# Start ssh-agent
eval "$(ssh-agent -s)"

# Add key to ssh-agent
ssh-add ~/.ssh/id_ed25519
```

### Step 2: Add SSH Key to GitHub
```bash
# Copy your public key
cat ~/.ssh/id_ed25519.pub
```

1. Go to GitHub → Settings → SSH and GPG keys
2. Click "New SSH key"
3. Paste your public key
4. Click "Add SSH key"

### Step 3: Connect Repository with SSH
```bash
# Add remote using SSH
git remote add origin git@github.com:USERNAME/battle-system.git

# Push
git branch -M main
git push -u origin main
```

## Option 3: Push to Different Account

If you want to push to a different GitHub account than your default:

### Method A: Use Personal Access Token (PAT)

1. **Create PAT on target account:**
   - Go to GitHub → Settings → Developer settings → Personal access tokens → Tokens (classic)
   - Click "Generate new token (classic)"
   - Select scopes: `repo` (full control)
   - Copy the token (you won't see it again!)

2. **Push using PAT:**
   ```bash
   # Add remote with token in URL
   git remote add origin https://TOKEN@github.com/USERNAME/battle-system.git
   
   # Or use credential helper
   git remote add origin https://github.com/USERNAME/battle-system.git
   git push -u origin main
   # When prompted for password, paste your PAT
   ```

### Method B: Configure Git for Specific Repository

```bash
# Set user for this repository only
git config user.name "Other Account Name"
git config user.email "other@email.com"

# Verify
git config user.name
git config user.email

# Add remote and push
git remote add origin https://github.com/OTHER_USERNAME/battle-system.git
git push -u origin main
```

## Continuous Pushing During Ralph Wiggum Loop

### Option 1: Manual Push After Each Iteration
After the loop makes commits, push them:
```bash
git push
```

### Option 2: Auto-Push in Loop Script
Modify `ralph-wiggum-loop.sh` to automatically push after each iteration.

Add this after the commit section (around line 120):
```bash
# Auto-push to remote (optional)
if git remote get-url origin &> /dev/null; then
    echo -e "${YELLOW}Pushing to remote...${NC}"
    git push origin main || echo -e "${RED}Push failed${NC}"
fi
```

## Common Issues & Solutions

### Issue: "Permission denied (publickey)"
**Solution:** Use HTTPS instead of SSH, or set up SSH keys properly

### Issue: "Authentication failed"
**Solution:** 
- Use Personal Access Token instead of password
- GitHub no longer accepts passwords for git operations

### Issue: "Remote already exists"
**Solution:**
```bash
# Remove existing remote
git remote remove origin

# Add new remote
git remote add origin https://github.com/USERNAME/battle-system.git
```

### Issue: "Updates were rejected"
**Solution:**
```bash
# Pull first, then push
git pull origin main --rebase
git push origin main
```

## Verify Your Setup

Run these commands to check everything:

```bash
# Check remote configuration
git remote -v

# Check current branch
git branch

# Check git user config
git config user.name
git config user.email

# Test connection (SSH only)
ssh -T git@github.com
```

## Best Practices

1. **Use SSH for convenience** - No need to enter credentials
2. **Use PAT for HTTPS** - More secure than passwords
3. **Set repository-specific config** - If using different accounts
4. **Push regularly** - Don't lose your work
5. **Use meaningful commit messages** - Ralph Wiggum loop does this automatically

## Quick Reference

```bash
# Add remote
git remote add origin URL

# Check remotes
git remote -v

# Push to remote
git push -u origin main

# Pull from remote
git pull origin main

# Remove remote
git remote remove origin

# Change remote URL
git remote set-url origin NEW_URL
```

## After Setup

Once your remote is configured, the Ralph Wiggum loop will:
1. Make local commits after each iteration
2. You can push manually: `git push`
3. Or modify the script to auto-push

Your battle system development will be safely backed up on GitHub! 🚀