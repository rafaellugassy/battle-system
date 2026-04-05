# Ralph Wiggum AI Coding Loop Setup

## What is the Ralph Wiggum Technique?

The Ralph Wiggum AI loop technique is an autonomous development methodology that repeatedly feeds a prompt to an AI coding agent until a task is completed. Named after the Simpsons character, it prioritizes **persistence over perfection**.

### Key Principles:
- **Persistent Iteration**: Runs until task passes verification
- **Failures as Data**: Each failed iteration provides learning data
- **Fresh Context**: Each iteration starts with clean slate
- **Persistent Memory**: Memory maintained via files (progress.txt, git history)

## Setup Instructions

### Prerequisites
- Bash shell (Linux/macOS/WSL)
- Git installed
- .NET SDK (for C# project)
- Claude Code or similar AI coding agent

### Files Created
1. **prompt.md** - Main task description and success criteria
2. **progress.txt** - Tracks completed items and next steps
3. **ralph-wiggum-loop.sh** - Main loop script
4. **battle-spec.md** - Technical specifications (already exists)
5. **battle-logic.md** - Battle resolution rules (already exists)

### Quick Start

1. **Make the script executable:**
   ```bash
   chmod +x ralph-wiggum-loop.sh
   ```

2. **Review the prompt:**
   ```bash
   cat prompt.md
   ```

3. **Run the loop:**
   ```bash
   ./ralph-wiggum-loop.sh
   ```

## How It Works

### Loop Flow
```
┌─────────────────────────────────────┐
│  Start Iteration N                  │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  Read progress.txt                  │
│  Read prompt.md                     │
│  Read spec files                    │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  AI Agent Processes Task            │
│  - Implements next step             │
│  - Runs tests                       │
│  - Updates progress.txt             │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  Commit Changes                     │
│  (Git tracks history)               │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  Check Success Criteria             │
│  - All tests passing?               │
│  - progress.txt shows complete?     │
└──────────────┬──────────────────────┘
               │
       ┌───────┴───────┐
       │               │
    Success?        Failure?
       │               │
       ▼               ▼
    [DONE]      [Next Iteration]
```

### Success Criteria
The loop completes when:
1. All unit tests pass (dotnet test succeeds)
2. progress.txt contains "Status: COMPLETE"
3. All models and battle logic implemented per spec

### Safety Features
- **Max Iterations**: Default 50 (prevents infinite loops)
- **Manual Control**: Press 'q' to quit any iteration
- **Git History**: Every iteration is committed
- **Logging**: All activity logged to ralph-wiggum.log

## Manual Execution (Current Setup)

Since this is a demonstration setup, the script currently requires manual execution:

1. Script pauses after each iteration
2. You manually implement the next step using your AI assistant
3. Press Enter when ready for next iteration
4. Script checks for changes and commits

## Automated Execution (Production Setup)

For full automation, you would:

1. **Integrate with Claude Code API:**
   ```bash
   # Replace manual prompt with API call
   claude-code --prompt "$(cat iteration_prompt.txt)" --workspace .
   ```

2. **Add automatic verification:**
   ```bash
   # Run tests automatically
   dotnet test --verbosity quiet
   ```

3. **Set up CI/CD integration:**
   - GitHub Actions
   - GitLab CI
   - Jenkins

## Monitoring Progress

### Check Current Status
```bash
cat progress.txt
```

### View Iteration History
```bash
git log --oneline
```

### Review Logs
```bash
tail -f ralph-wiggum.log
```

### See What Changed
```bash
git diff HEAD~1
```

## Best Practices

### 1. Define Clear Success Criteria
- Specific, measurable goals
- Automated verification when possible
- Document in prompt.md

### 2. Use progress.txt Effectively
- Update after each change
- Document blockers
- Track test results
- Note what to try next

### 3. Start Small
- Begin with simple tasks
- Build confidence in the system
- Gradually increase complexity

### 4. Set Guardrails
- Maximum iteration limits
- Cost monitoring (API usage)
- Manual checkpoints for critical changes

### 5. Maintain Clean Git History
- Descriptive commit messages
- Commit after each logical change
- Easy to rollback if needed

## Troubleshooting

### Loop Not Making Progress
- Check progress.txt for blockers
- Review recent git commits
- Manually verify the next step is clear
- Consider breaking task into smaller pieces

### Tests Failing Repeatedly
- Document specific test failures in progress.txt
- Provide more context in prompt.md
- Consider implementing tests incrementally

### Infinite Loop Risk
- Monitor iteration count
- Set conservative MAX_ITERATIONS
- Use manual mode for complex tasks
- Review logs regularly

## Project Structure

```
/Wiggim test
├── battle-logic.md          # Battle resolution rules
├── battle-spec.md           # Technical specifications
├── prompt.md                # Main task prompt
├── progress.txt             # Progress tracking
├── ralph-wiggum-loop.sh     # Loop script
├── ralph-wiggum.log         # Execution log
├── README-RALPH-WIGGUM.md   # This file
└── src/                     # Will be created
    ├── models/              # C# model classes
    └── tests/               # Unit tests
```

## Next Steps

1. **Review the specifications:**
   - Read battle-spec.md
   - Read battle-logic.md
   - Understand the task scope

2. **Customize prompt.md if needed:**
   - Add specific requirements
   - Adjust success criteria
   - Set priorities

3. **Run the loop:**
   ```bash
   ./ralph-wiggum-loop.sh
   ```

4. **Monitor and guide:**
   - Check progress.txt regularly
   - Review commits
   - Provide feedback in progress.txt

## References

- Ralph Wiggum Technique: Autonomous iterative development
- Inspired by: Continuous integration and test-driven development
- Philosophy: "I'm helping!" - Persistent, iterative improvement

## Support

For issues or questions:
1. Check ralph-wiggum.log for errors
2. Review progress.txt for blockers
3. Examine git history for what changed
4. Adjust prompt.md for clarity

---

**Remember**: The Ralph Wiggum technique is about persistence and iteration. Each failure is data for the next attempt. Keep the loop running until success! 🎉