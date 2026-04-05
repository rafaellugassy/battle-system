#!/bin/bash

# Ralph Wiggum AI Coding Loop
# Autonomous iterative development using Claude Code

set -e

# Configuration
MAX_ITERATIONS=50
ITERATION=0
SUCCESS=false
PROMPT_FILE="prompt.md"
PROGRESS_FILE="progress.txt"
LOG_FILE="ralph-wiggum.log"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Initialize log
echo "=== Ralph Wiggum Loop Started at $(date) ===" > "$LOG_FILE"

# Function to check if task is complete
check_success() {
    echo -e "${BLUE}Checking success criteria...${NC}"
    
    # Check if all tests exist and pass
    if [ -d "src/tests" ]; then
        # Check if we can build and run tests
        if command -v dotnet &> /dev/null; then
            if dotnet test --no-build --verbosity quiet 2>&1 | grep -q "Passed!"; then
                echo -e "${GREEN}✓ All tests passing${NC}"
                return 0
            fi
        fi
    fi
    
    # Check progress file for completion markers
    if grep -q "Status: COMPLETE" "$PROGRESS_FILE" 2>/dev/null; then
        echo -e "${GREEN}✓ Progress file indicates completion${NC}"
        return 0
    fi
    
    echo -e "${YELLOW}✗ Task not yet complete${NC}"
    return 1
}

# Function to run single iteration
run_iteration() {
    local iter=$1
    echo -e "\n${BLUE}========================================${NC}"
    echo -e "${BLUE}Iteration $iter of $MAX_ITERATIONS${NC}"
    echo -e "${BLUE}========================================${NC}\n"
    
    # Log iteration start
    echo "--- Iteration $iter started at $(date) ---" >> "$LOG_FILE"
    
    # Read current progress
    if [ -f "$PROGRESS_FILE" ]; then
        echo -e "${YELLOW}Current Progress:${NC}"
        head -n 10 "$PROGRESS_FILE"
        echo ""
    fi
    
    # Create iteration prompt
    cat > iteration_prompt.txt <<EOF
You are in iteration $iter of an autonomous development loop (Ralph Wiggum technique).

Read the following files to understand your task:
1. prompt.md - Main task description and success criteria
2. progress.txt - What has been completed and what's next
3. battle-spec.md - Technical specifications
4. battle-logic.md - Battle resolution rules

Your goal this iteration:
- Read progress.txt to see what's been done
- Complete the next logical step in the implementation
- Update progress.txt with what you accomplished
- Commit your changes with a descriptive message
- If you encounter errors, document them in progress.txt for the next iteration

IMPORTANT: 
- Make incremental progress - don't try to do everything at once
- Update progress.txt after each change
- Commit frequently with clear messages
- If tests fail, document the failure and fix in next iteration
EOF

    echo -e "${YELLOW}Iteration prompt created${NC}"
    echo "Iteration $iter prompt:" >> "$LOG_FILE"
    cat iteration_prompt.txt >> "$LOG_FILE"
    
    # In a real implementation, this would call Claude Code API
    # For now, we'll provide instructions for manual execution
    echo -e "\n${GREEN}==> Ready for AI agent to process${NC}"
    echo -e "${GREEN}==> In a real setup, this would automatically invoke Claude Code${NC}"
    echo -e "${GREEN}==> For manual execution: Review iteration_prompt.txt and execute the task${NC}\n"
    
    # Simulate waiting for completion
    echo -e "${YELLOW}Press Enter when iteration is complete, or 'q' to quit:${NC}"
    read -r response
    
    if [ "$response" = "q" ]; then
        echo -e "${RED}Loop terminated by user${NC}"
        return 1
    fi
    
    # Check if git changes were made
    if git diff --quiet && git diff --cached --quiet; then
        echo -e "${YELLOW}⚠ No changes detected - AI may need guidance${NC}"
    else
        echo -e "${GREEN}✓ Changes detected${NC}"
        git add -A
        git commit -m "Ralph Wiggum Loop - Iteration $iter" || true
    fi
    
    echo "Iteration $iter completed at $(date)" >> "$LOG_FILE"
    return 0
}

# Main loop
echo -e "${GREEN}╔════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║   Ralph Wiggum AI Coding Loop v1.0    ║${NC}"
echo -e "${GREEN}║   'I'm helping!' - Ralph Wiggum        ║${NC}"
echo -e "${GREEN}╚════════════════════════════════════════╝${NC}\n"

echo -e "${BLUE}Configuration:${NC}"
echo -e "  Max Iterations: $MAX_ITERATIONS"
echo -e "  Prompt File: $PROMPT_FILE"
echo -e "  Progress File: $PROGRESS_FILE"
echo -e "  Log File: $LOG_FILE\n"

# Initialize git if needed
if [ ! -d ".git" ]; then
    echo -e "${YELLOW}Initializing git repository...${NC}"
    git init
    git add .
    git commit -m "Initial commit - Ralph Wiggum Loop setup" || true
fi

# Main iteration loop
while [ $ITERATION -lt $MAX_ITERATIONS ]; do
    ITERATION=$((ITERATION + 1))
    
    # Run iteration
    if ! run_iteration $ITERATION; then
        echo -e "${RED}Loop terminated${NC}"
        break
    fi
    
    # Check success
    if check_success; then
        SUCCESS=true
        echo -e "\n${GREEN}╔════════════════════════════════════════╗${NC}"
        echo -e "${GREEN}║          SUCCESS! 🎉                   ║${NC}"
        echo -e "${GREEN}║   Task completed in $ITERATION iterations    ║${NC}"
        echo -e "${GREEN}╚════════════════════════════════════════╝${NC}\n"
        break
    fi
    
    # Brief pause between iterations
    sleep 1
done

# Final summary
echo -e "\n${BLUE}========================================${NC}"
echo -e "${BLUE}Ralph Wiggum Loop Summary${NC}"
echo -e "${BLUE}========================================${NC}"
echo -e "Total Iterations: $ITERATION"
echo -e "Status: $([ "$SUCCESS" = true ] && echo -e "${GREEN}SUCCESS${NC}" || echo -e "${YELLOW}INCOMPLETE${NC}")"
echo -e "Log File: $LOG_FILE"
echo -e "\n${YELLOW}Final Progress:${NC}"
cat "$PROGRESS_FILE"

echo -e "\n=== Ralph Wiggum Loop Ended at $(date) ===" >> "$LOG_FILE"

if [ "$SUCCESS" = true ]; then
    exit 0
else
    exit 1
fi

# Made with Bob
