# Running Tests on GitHub Actions (Free CI/CD Server)

GitHub Actions will run your tests on a server and show you ALL the output, including your `_output.WriteLine()` debug statements!

## Setup Steps:

### 1. Create a GitHub Repository

```bash
# If you haven't already, initialize git
git init

# Add all files
git add .

# Commit
git commit -m "Initial commit - Battle system with 106 tests"

# Create a new repository on GitHub.com, then:
git remote add origin https://github.com/YOUR_USERNAME/YOUR_REPO_NAME.git
git branch -M main
git push -u origin main
```

### 2. GitHub Actions Will Run Automatically

Once you push to GitHub, the workflow will run automatically because we created `.github/workflows/test.yml`.

### 3. View Test Output

1. Go to your repository on GitHub.com
2. Click the **"Actions"** tab at the top
3. Click on the latest workflow run
4. Click on the **"test"** job
5. Expand the steps to see output:
   - **"Run ALL tests with detailed output"** - Shows all 106 tests
   - **"Run Multi-Turn tests specifically"** - Shows just the 6 multi-turn tests with your debug output!

### 4. Manual Trigger

You can also manually trigger the tests:
1. Go to **Actions** tab
2. Click **"Run Tests with Output"** workflow
3. Click **"Run workflow"** button
4. Select branch and click **"Run workflow"**

## What You'll See:

The GitHub Actions output will show:

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

## Alternative: Use the Provided Script

If you want to push to GitHub quickly:

```bash
# Make the script executable
chmod +x push-to-github.sh

# Run it (it will prompt for your GitHub username and repo name)
./push-to-github.sh
```

## Benefits of GitHub Actions:

✅ **Free** - Unlimited for public repositories
✅ **Full output** - See all test results and debug statements
✅ **Automatic** - Runs on every push
✅ **History** - Keep track of all test runs
✅ **Shareable** - Anyone can see the test results

## Troubleshooting:

**If .NET 10.0 isn't available yet:**
Edit `.github/workflows/test.yml` and change:
```yaml
dotnet-version: '10.0.x'
```
to:
```yaml
dotnet-version: '8.0.x'
```

Then update `src/BattleSystem.csproj` to use `net8.0` instead of `net10.0`.

## Example Output Location:

After pushing to GitHub, your test output will be at:
```
https://github.com/YOUR_USERNAME/YOUR_REPO_NAME/actions
```

Click on any workflow run to see the full test output with all your debug statements!