# Battle Logic Reference
Version: 1.0

---

## Overview

A battle is a single round between two players. Each player brings one card. The battle resolves in a fixed sequence of steps. All effects fully resolve before the winner is determined.

---

## Objects

---

### Card
| Property | Type | Description |
|---|---|---|
| id | string | Unique identifier |
| name | string | Display name |
| color | string | `red`, `blue`, `yellow`, `green`, `colorless` |
| strength | integer | Base damage value |
| zones | Zone[4] | One zone per color |
| powers | Power[] | One or more powers |

- A card must have exactly 4 zones, one per color
- A card must have at least one power
- A `colorless` card generates no mana after battle
- Not all cards deal strength damage — defensive cards may rely entirely on their powers

---

### Zone
| Property | Type | Description |
|---|---|---|
| color | string | `red`, `blue`, `yellow`, `green` |
| state | string | `attack`, `defend`, `empty` |

- `attack` — attempts to deal damage to the opposing card's matching color zone
- `defend` — nullifies an incoming attack from the opposing card's matching color zone
- `empty` — inactive, does nothing
- Zone states are fixed on the card and do not change during play

---

### Power
| Property | Type | Description |
|---|---|---|
| id | string | Unique identifier |
| condition | Condition | When this power activates |
| effect | Effect | What this power does |
| manaCost | ManaCost or null | Mana consumed on activation |
| manaRequirement | ManaRequirement or null | Mana that must be present but is not consumed |

#### Effect Types
| Type | Description |
|---|---|
| `ABILITY_DAMAGE` | Replaces the card's strength damage. Maximum one per card |
| `ADDITIONAL_DAMAGE` | Deals damage after the main attack resolves |
| `APPLY_STATUS` | Applies a status to a target player |
| `HEAL` | Heals HP on a target player |
| `HIT_TWICE` | Causes the card's attack to resolve twice |

---

### Condition
| Type | When It Triggers |
|---|---|
| `ON_HIT` | When this card's attack resolves as a hit (per Greatest Hit Rule). Fires once per resolution, twice if HIT_TWICE is active |
| `ON_DEFEND` | When this card's defend zone successfully nullifies an incoming attack. Fires per defend zone that blocks |
| `ON_BEING_HIT` | When this card is hit by an opponent attack zone |
| `AFTER_TURN_RESOLVES` | After all zone resolution for the turn is complete |

---

### Mana
| Property | Type | Description |
|---|---|---|
| color | string | `empty`, `red`, `blue`, `yellow`, `green` |
| receivedAt | integer | Insertion order index, used to determine age |

- Each player has 10 mana slots, all starting as `empty`
- Mana persists between battles
- After each battle, a player gains 1 mana matching their card's color (colorless cards generate nothing)
- If all 10 slots are full, the oldest mana is removed before adding the new one
- When spending colored mana, oldest of that color is consumed first
- When spending `any` mana, the overall oldest mana is consumed first

#### ManaCost
- Defines mana that is **consumed** when a power activates
- Specified per color with an amount (e.g. `{ color: "blue", amount: 2 }`)
- Color can be `any`
- If the player cannot meet the cost, the power does not activate

#### ManaRequirement
- Defines mana that must be **present** but is not consumed
- Same structure as ManaCost
- If the requirement is not met, the power does not activate
- When both a cost and requirement exist on one power, check the requirement first, then spend the cost. If either fails, the power does not activate and no mana is spent

---

### Player
| Property | Type | Description |
|---|---|---|
| id | string | Unique identifier |
| hp | integer | Current HP. Starts at 20. Eliminated at 0 |
| maxHp | integer | Maximum HP. Starts at 20. Modified by abilities or statuses |
| mana | Mana[10] | 10 mana slots |
| statuses | Status[] | Active statuses in order of receipt |
| card | Card | The card fielded this battle |
| isEliminated | boolean | Set to true when HP reaches 0 or below |

- Once `isEliminated` is true the player cannot act, resolve powers, receive healing, or tick statuses
- If maxHp is increased by an effect, it returns to its prior value when that effect ends, unless another effect is still active

---

### Status
| Property | Type | Description |
|---|---|---|
| type | string | The status type |
| duration | integer | Battles remaining. Decremented each battle. Removed at 0 |
| power | integer or null | Effect strength for statuses that use it |
| receivedAt | integer | Order received, used for resolution order |

#### Status Types
| Type | Effect |
|---|---|
| `slow` | This player acts second regardless of turn order |
| `quick` | This player acts first regardless of turn order |
| `silence` | This player's powers cannot activate. Mana is neither checked nor spent |
| `protect` | Damage taken is halved. Does not apply to burn or status damage |
| `weaken` | Damage dealt is halved. Does not apply to additional damage or burn |
| `vulnerable` | Damage taken is doubled. Does not apply to burn or status damage |
| `weakness(color)` | Damage taken from a card of the specified color is doubled. Does not apply to burn or status damage |
| `burn` | Player takes damage equal to `power` each turn. Unaffected by all damage modifiers |
| `renew` | Player heals equal to `power` each turn. Cannot exceed maxHp |

- Multiple statuses of the same type can be active simultaneously, each tracked independently
- The same status applied more than once creates separate instances with their own duration and power
- Statuses resolve in order of receipt
- Statuses only tick for players still in the battle

---

## Battle Resolution Steps

---

### Step 1 — Determine Acting Order

1. Count `quick` and `slow` stacks for each player
2. Resolve conflicts:

| Scenario | Result |
|---|---|
| One player has `quick`, other has `slow` | `quick` player acts first |
| Both players have `quick` | Use pre-assigned turn order |
| Both players have `slow` | Use pre-assigned turn order |
| Player A has N `slow`, Player B has 1 `quick` | `quick` cancels one `slow`. If A still has remaining `slow`, A acts second. Status instances retain their own durations regardless |
| Player A has N `quick`, Player B has 1 `slow` | `slow` cancels one `quick`. If A still has remaining `quick`, A acts first. Status instances retain their own durations regardless |

3. Set acting order as [firstPlayer, secondPlayer]

---

### Step 2 — First Player Attack Resolution

1. Evaluate all 4 zones of the first player's card against the second player's card
2. Apply the **Greatest Hit Rule** to find the single best result:
   - If any attacking zone faces an `empty` opposing zone → `HIT_EMPTY`
   - Else if any attacking zone faces an `attack` opposing zone → `HIT_ATTACK`
   - Else → `NO_HIT`
3. Resolve damage:
   - `HIT_EMPTY` → deal full strength (or `ABILITY_DAMAGE` value if present) to second player
   - `HIT_ATTACK` → deal half strength (or `ABILITY_DAMAGE` value) to second player. Use floor rounding
   - `NO_HIT` → no damage
4. Apply **Damage Modifiers** (see below)
5. Check if second player is eliminated. Set `isEliminated` if HP <= 0
6. Fire `ON_HIT` powers for first player if result is `HIT_EMPTY` or `HIT_ATTACK`:
   - Fires once normally
   - Fires twice if a `HIT_TWICE` power is active (second hit uses same damage value and same hit type)
   - If `ABILITY_DAMAGE` and `ADDITIONAL_DAMAGE` both trigger, ability damage resolves first, then additional damage
7. For each `defend` zone on the first player's card that successfully blocked an incoming attack, fire `ON_DEFEND` powers
8. For each zone on the first player's card that was hit by an opponent attack (zone is `empty` or `attack` and opponent zone is `attack`), fire `ON_BEING_HIT` powers
9. After each effect resolves, check eliminations

---

### Step 3 — Second Player Attack Resolution

1. If second player `isEliminated`, skip this step entirely
2. Repeat Step 2 with players reversed
3. Note: if first player becomes eliminated during their own turn (e.g. self-damage from a power), second player still takes their turn

---

### Step 4 — After Turn Resolves

1. For each player still in the battle, fire all `AFTER_TURN_RESOLVES` powers in order of receipt
2. Evaluate mana costs and requirements at the moment each power fires
3. Apply effects
4. Check eliminations after each effect resolves

---

### Step 5 — Burn and Renew

1. For each player still in the battle, resolve all `burn` and `renew` statuses in order of receipt
2. `burn` deals damage equal to `power`. Unaffected by all damage modifiers including protect, vulnerable, weaken, and weakness(color)
3. `renew` heals HP equal to `power`. Cannot exceed maxHp. If already at maxHp, does nothing
4. Check eliminations after each burn tick

---

### Step 6 — Status Duration Decrement

1. For each player still in the battle, decrement all status durations by 1
2. Remove any status whose duration has reached 0

---

### Step 7 — Determine Winner

| State | Result |
|---|---|
| Both players eliminated | `draw` |
| Only player1 eliminated | `player2 wins` |
| Only player2 eliminated | `player1 wins` |
| Neither eliminated | No winner this round, continue to next battle |

---

### Step 8 — Mana Generation

1. For each player, if their card is not `colorless`, add 1 mana of that color to their pool
2. If the pool is full (10 slots), remove the oldest mana first, then add the new mana

---

## Greatest Hit Rule

A card may have multiple attack zones. Only the **single best result** across all zones counts for damage and On Hit power activation.

### Priority
1. `attack` vs `empty` → `HIT_EMPTY` (full damage, ON_HIT fires)
2. `attack` vs `attack` → `HIT_ATTACK` (half damage, ON_HIT fires)
3. All attacks blocked or no attack zones → `NO_HIT` (no damage, ON_HIT does not fire)

### Rules
- Even if multiple zones produce the same result (e.g. two zones both hitting empty), it still counts as one hit
- Only the powers and damage of that single best hit resolve
- ON_DEFEND fires independently per defend zone that blocks, regardless of the Greatest Hit result

---

## Damage Modifier Resolution

### Order of Operations
1. Flat modifiers apply first
2. All multiplicative modifiers apply simultaneously

### Modifier Table
| Modifier | Source | Strength Damage | Ability Damage | Additional Damage | Burn / Status Damage |
|---|---|---|---|---|---|
| `weaken` | Attacker status | x0.5 | x0.5 | No effect | No effect |
| `protect` | Defender status | x0.5 | x0.5 | x0.5 | No effect |
| `vulnerable` | Defender status | x2 | x2 | x2 | No effect |
| `weakness(color)` | Defender status | x2 if card color matches | x2 if card color matches | x2 if card color matches | No effect |

### Simultaneous Application
All active multipliers are multiplied together at the same time.

Example:
- Attacker (red card) has `weaken`
- Defender has `vulnerable` and `weakness(red)`
- Base strength damage: 6
- Multipliers: x0.5 (weaken) x x2 (vulnerable) x x2 (weakness(red)) = x2 net
- Final damage: 6 x 2 = 12

---

## Post-Death Resolution

- When a player's HP reaches 0 or below, they are immediately marked `isEliminated = true`
- The current action always finishes resolving (e.g. if HIT_TWICE is active and first hit eliminates the opponent, the second hit still resolves)
- Eliminated players cannot act, resolve powers, receive healing, or tick statuses for the remainder of the battle
- If player2 is eliminated during player1's turn, player2's attack turn (Step 3) is skipped
- If player1 is eliminated during their own turn from self-damage, player2 still takes their attack turn in Step 3
- Winner is not determined until Step 7 — burn in Step 5 can still eliminate the surviving player, resulting in a draw

---

## Edge Cases

### Both Players Die Simultaneously
- If a power deals damage to both players at once and both reach 0 HP → draw
- If both players die to burn in Step 5 → draw
- Winner is always determined at Step 7 after all resolution is complete

### Protect and Vulnerable on Same Player
- Both apply simultaneously: x0.5 x x2 = x1.0 net (no change)

### Multiple Weakness Stacks
- Each stack applies its own x2 multiplier simultaneously
- Two weakness(red) stacks on defender hit by red card: x2 x x2 = x4

### Silence and Mana
- Silenced player's powers do not fire
- Mana is neither checked nor spent for any power while silenced

### Renew on Eliminated Player
- Once a player is eliminated, renew does not fire for them

### Mana Cost Failure Mid-Power
- If a power has both a mana requirement and mana cost, check requirement first, then spend cost
- If either check fails, the power does not activate and no mana is spent

### HIT_TWICE and Additional Damage
- If a card has HIT_TWICE and ADDITIONAL_DAMAGE both triggered by ON_HIT:
  - First hit: ability or strength damage resolves, then additional damage resolves
  - Second hit: same sequence repeats

### Dead Player's After Turn Resolves Powers
- If a player is eliminated before Step 4, their AFTER_TURN_RESOLVES powers do not fire

### Damage Rounding
- All non-integer damage results use floor rounding
- Example: strength 5 on HIT_ATTACK = floor(2.5) = 2
