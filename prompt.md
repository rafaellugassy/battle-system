# Battle System Implementation Task

## Objective
Implement a complete C# battle system based on the specifications in `battle-spec.md` and `battle-logic.md`.

## Success Criteria
1. All models implemented according to spec (Card, Zone, Power, Condition, Player, Mana, Status, Battle)
2. All unit tests from battle-spec.md implemented and passing
3. Battle resolution logic correctly implements all 8 steps
4. All edge cases handled as specified
5. Code follows C# best practices and conventions

## Current Status
Check `progress.txt` for completed items.

## Implementation Order
1. Create project structure (/src/models, /src/tests)
2. Implement basic models (Zone, Mana, Status)
3. Implement Card and Power models
4. Implement Player model
5. Implement Battle resolution logic
6. Implement all unit tests
7. Verify all tests pass

## Reference Files
- `battle-spec.md`: Complete technical specification with models and tests
- `battle-logic.md`: Detailed battle resolution rules and edge cases

## Notes
- Use floor rounding for all non-integer damage calculations
- Process all status and power resolution in order of receipt
- Check isEliminated flag after every damage-dealing operation
- Mana age tracked by insertion order (index 0 = oldest)