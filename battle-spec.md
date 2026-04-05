# Battle System Specification
Version: 1.0

---

## Project Structure

```
/src
  /models
    Card.cs
    Zone.cs
    Power.cs
    Condition.cs
    Player.cs
    Mana.cs
    Status.cs
    Battle.cs
  /tests
    Card.test.cs
    Zone.test.cs
    Power.test.cs
    Condition.test.cs
    Player.test.cs
    Mana.test.cs
    Status.test.cs
    Battle.test.cs
```

---

## Models

---

### Zone.cs

#### Description
Represents one of four colored slots on a card. Each zone has a fixed color and a fixed state.

#### Properties
| Property | Type | Values |
|---|---|---|
| color | string | `"red"`, `"blue"`, `"yellow"`, `"green"` |
| state | string | `"attack"`, `"defend"`, `"empty"` |

#### Rules
- Color and state are both immutable after construction
- Each card will always have exactly one zone per color

---

### Card.cs

#### Description
Represents a card fielded by a player in a battle. All properties are fixed and defined at construction.

#### Properties
| Property | Type | Values |
|---|---|---|
| id | string | Unique identifier |
| name | string | Display name |
| color | string | `"red"`, `"blue"`, `"yellow"`, `"green"`, `"colorless"` |
| strength | integer | Any positive integer |
| zones | Zone[4] | Exactly one zone per color |
| powers | Power[] | One or more powers |

#### Rules
- Must have exactly 4 zones, one per color
- Must have at least one power
- `colorless` cards generate no mana for their player after battle
- Not all cards will deal strength damage — some are purely defensive and rely on their powers

---

### Power.cs

#### Description
Represents an ability on a card. Each power has a condition that determines when it fires, optional mana interactions, and an effect.

#### Properties
| Property | Type | Description |
|---|---|---|
| id | string | Unique identifier |
| condition | Condition | When this power activates |
| effect | Effect | What this power does when activated |
| manaCost | ManaCost \| null | Mana consumed on activation |
| manaRequirement | ManaRequirement \| null | Mana that must be present but is not consumed |

#### Effect Types
| Effect | Description |
|---|---|
| `ABILITY_DAMAGE` | Replaces the card's strength damage. Only one per card |
| `ADDITIONAL_DAMAGE` | Deals damage after the main attack resolves |
| `APPLY_STATUS` | Applies a status to a target player |
| `HEAL` | Heals HP on a target player |
| `HIT_TWICE` | Causes the card's attack to hit twice |

#### Rules
- A card may only have one `ABILITY_DAMAGE` power
- A card may have multiple `ADDITIONAL_DAMAGE`, `APPLY_STATUS`, `HEAL`, or `HIT_TWICE` powers
- Mana cost and mana requirement are both evaluated at the moment the condition is checked
- If a player is silenced, no powers activate and mana is neither checked nor spent
- If a condition is not met, the power does not activate and any associated additional damage does not trigger

---

### ManaCost.cs

#### Description
Defines mana that is consumed when a power activates.

#### Properties
| Property | Type | Description |
|---|---|---|
| costs | CostEntry[] | Array of color and amount pairs |

#### CostEntry
| Property | Type | Description |
|---|---|---|
| color | string | `"red"`, `"blue"`, `"yellow"`, `"green"`, `"any"` |
| amount | integer | How many of this color to consume |

#### Rules
- When spending colored mana, the oldest mana of that color is consumed first
- When spending `"any"` mana, the oldest mana regardless of color is consumed first
- If the player does not have enough mana to meet the cost, the power does not activate

---

### ManaRequirement.cs

#### Description
Defines mana that must be present for a power to activate but is not consumed.

#### Properties
| Property | Type | Description |
|---|---|---|
| requirements | RequirementEntry[] | Array of color and amount pairs |

#### RequirementEntry
| Property | Type | Description |
|---|---|---|
| color | string | `"red"`, `"blue"`, `"yellow"`, `"green"`, `"any"` |
| amount | integer | How many of this color must be present |

#### Rules
- Mana is not spent, only checked
- If the requirement is not met, the power does not activate

---

### Condition.cs

#### Description
Defines when a power activates during battle resolution.

#### Types
| Type | Trigger |
|---|---|
| `ON_HIT` | Activates when this card's attack resolves as a hit per the Greatest Hit Rule |
| `ON_DEFEND` | Activates when this card's defend zone successfully nullifies an incoming attack |
| `ON_BEING_HIT` | Activates when an opponent's attack zone hits this card |
| `AFTER_TURN_RESOLVES` | Activates after all attack and defend resolution for the turn is complete |

#### Rules
- `ON_HIT` fires once per attack resolution, not once per attacking zone
- `ON_HIT` fires twice if the card is hitting twice (HIT_TWICE power active)
- `ON_DEFEND` fires independently for each defend zone that successfully blocks
- If the condition is not met, powers with that condition do not activate

---

### Mana.cs

#### Description
Represents a single mana slot for a player.

#### Properties
| Property | Type | Values |
|---|---|---|
| color | string | `"empty"`, `"red"`, `"blue"`, `"yellow"`, `"green"` |
| receivedAt | integer | Battle index when this mana was received, used to determine age |

#### Rules
- Each player has exactly 10 mana slots
- All slots begin as `"empty"` at game start
- Mana persists between battles
- After each battle resolves, a player gains 1 mana matching their played card's color, unless the card is `colorless`
- If all 10 slots are full, the oldest mana is removed to make room for the new mana
- When spending colored mana, the oldest mana of that color is consumed first
- When spending `"any"` mana, the oldest mana regardless of color is consumed first

---

### Status.cs

#### Description
Represents a status effect applied to a player. Each instance is independent even if the same type is applied multiple times.

#### Properties
| Property | Type | Description |
|---|---|---|
| type | string | The status type |
| duration | integer | Turns remaining. Decremented by 1 each battle. Removed at 0 |
| power | integer \| null | Strength of the effect for statuses that use it |
| receivedAt | integer | Order in which this status was received, used for resolution order |

#### Status Types
| Type | Effect |
|---|---|
| `slow` | This player's card acts second regardless of turn order |
| `quick` | This player's card acts first regardless of turn order |
| `silence` | This player's powers cannot activate. Mana is neither checked nor spent |
| `protect` | Damage taken is halved. Does not apply to burn or status damage |
| `weaken` | Damage dealt is halved. Does not apply to additional damage or burn |
| `vulnerable` | Damage taken is doubled. Does not apply to burn or status damage |
| `weakness(color)` | Damage taken from a card of the specified color is doubled. Does not apply to burn or status damage |
| `burn` | Player takes damage equal to `power` each turn. Not affected by any damage modifiers |
| `renew` | Player heals HP equal to `power` each turn. Cannot exceed max HP |

#### Rules
- Multiple statuses of the same type can be active simultaneously, each tracked independently
- Statuses resolve in order of receipt
- Statuses only tick and resolve for players still in the battle (not eliminated)
- `weakness(color)` includes the color as part of the status (e.g. `weakness(red)`, `weakness(blue)`)
- Multiple `weakness` stacks of the same color each apply their own x2 multiplier

#### quick / slow Conflict Resolution
| Scenario | Result |
|---|---|
| One player has `quick`, other has `slow` | `quick` player goes first |
| Both players have `quick` | Default to pre-assigned turn order |
| Both players have `slow` | Default to pre-assigned turn order |
| One player has multiple `slow`, other has one `quick` | `quick` cancels one `slow` for turn order only. All status instances retain their own durations |
| One player has multiple `quick`, other has one `slow` | `slow` cancels one `quick` for turn order only. All status instances retain their own durations |

---

### Player.cs

#### Description
Represents a player participating in a battle.

#### Properties
| Property | Type | Description |
|---|---|---|
| id | string | Unique identifier |
| hp | integer | Current HP. Starts at 20. Min 0. Max 20 unless modified |
| maxHp | integer | Maximum HP. Starts at 20. Returns to base when modifying effect ends |
| mana | Mana[10] | Array of 10 mana slots |
| statuses | Status[] | Active statuses in order of receipt |
| card | Card | The card fielded this battle |
| isEliminated | boolean | True when HP reaches 0 or below |

#### Rules
- HP starts at 20, cannot exceed maxHp, cannot go below 0 for tracking purposes though elimination occurs at 0
- Once `isEliminated` is true the player cannot act, resolve powers, receive healing, or tick statuses
- If a status or ability raises maxHp, it returns to its prior value when that effect ends unless another effect is also active

---

### Battle.cs

#### Description
Manages the full resolution of a single battle between two players.

#### Properties
| Property | Type | Description |
|---|---|---|
| player1 | Player | First player |
| player2 | Player | Second player |
| turnOrder | Player[] | Pre-assigned order [first, second] |
| result | string \| null | `"player1"`, `"player2"`, `"draw"`, or `null` if ongoing |

---

## Battle Resolution

### Step 1 — Determine Acting Order
1. Collect all `quick` and `slow` statuses for both players
2. Count net `quick` vs `slow` stacks per player
3. Apply conflict resolution rules (see Status.cs quick/slow rules)
4. Set `actingOrder` as [firstPlayer, secondPlayer] for this battle

### Step 2 — First Player Attack Resolution
1. Evaluate all 4 zones of the first player's card against the second player's card
2. Apply the Greatest Hit Rule to determine the single best result:
   - If any attack zone hits an `empty` opposing zone → result is `HIT_EMPTY`
   - Else if any attack zone hits an `attack` opposing zone → result is `HIT_ATTACK`
   - Else → result is `NO_HIT`
3. Resolve damage based on result:
   - `HIT_EMPTY` → deal full strength (or ability damage if card has `ABILITY_DAMAGE` power) to second player
   - `HIT_ATTACK` → deal half strength (or ability damage) to second player
   - `NO_HIT` → no damage dealt
4. Apply damage modifiers (see Damage Modifier Resolution)
5. Check if second player is eliminated
6. Fire `ON_HIT` powers for first player if result is `HIT_EMPTY` or `HIT_ATTACK` (once, or twice if `HIT_TWICE` is active)
7. For each defend zone on the first player's card that successfully blocks an incoming attack, fire `ON_DEFEND` powers
8. For each zone on the first player's card that is hit by an opponent attack, fire `ON_BEING_HIT` powers

### Step 3 — Second Player Attack Resolution
1. If second player `isEliminated`, skip this step entirely
2. Repeat Step 2 with players reversed

### Step 4 — After Turn Resolves
1. For each player still in the battle, in order of power receipt, activate all `AFTER_TURN_RESOLVES` powers
2. Apply mana costs, requirements, and effects as each power resolves
3. Check eliminations after each effect

### Step 5 — Burn and Renew
1. For each player still in the battle, resolve all `burn` and `renew` statuses in order of receipt
2. `burn` deals damage equal to status `power`. Not affected by any damage modifiers
3. `renew` heals HP equal to status `power`. Cannot exceed maxHp
4. Check eliminations after each burn tick

### Step 6 — Status Duration Decrement
1. For each player still in the battle, decrement all status durations by 1
2. Remove any status whose duration has reached 0

### Step 7 — Determine Winner
1. If both players are eliminated → result is `"draw"`
2. If only player1 is eliminated → result is `"player2"`
3. If only player2 is eliminated → result is `"player1"`
4. If neither is eliminated → no winner yet, battle continues to next round

### Step 8 — Mana Generation
1. For each player, if their card's color is not `colorless`, add 1 mana of that color to their mana pool
2. If the player's mana pool is full (10 slots), remove the oldest mana before adding the new one

---

## Greatest Hit Rule

### Description
A card may have multiple attack zones. Only the single best result across all zones determines damage and On Hit power activation.

### Resolution
1. Check all zones where this card has `attack` state
2. For each attacking zone, check the opposing card's matching color zone
3. Collect all results
4. Select the best single result using priority:
   - Priority 1: Any `attack` vs `empty` → `HIT_EMPTY`
   - Priority 2: Any `attack` vs `attack` → `HIT_ATTACK`
   - Priority 3: All attacks blocked or no attack zones → `NO_HIT`
5. Even if multiple zones produce the same result, it still counts as one hit of that type

---

## Damage Modifier Resolution

### Order of Operations
1. Apply flat modifiers first
2. Apply all multiplicative modifiers simultaneously

### Modifier Table
| Modifier | Applies To | Does Not Apply To |
|---|---|---|
| `weaken` on attacker (x0.5) | Strength damage, Ability damage | Additional damage, Burn, Status damage |
| `protect` on defender (x0.5) | Strength damage, Ability damage, Additional damage | Burn, Status damage |
| `vulnerable` on defender (x2) | Strength damage, Ability damage, Additional damage | Burn, Status damage |
| `weakness(color)` on defender (x2) | Strength damage, Ability damage, Additional damage from matching color card | Burn, Status damage |

### Simultaneous Multiplier Example
- Attacker has `weaken`, defender has `vulnerable` and `weakness(red)`, attacking card is red
- Base damage: 4
- Multipliers: x0.5 (weaken) x x2 (vulnerable) x x2 (weakness(red)) = x2 net
- Final damage: 4 x 2 = 8

---

## Post-Death Resolution Rules

- Once a player reaches 0 HP they are immediately marked `isEliminated = true`
- Eliminated players cannot act, resolve powers, receive healing, or tick statuses
- If the second player is eliminated during the first player's turn, step 3 is skipped
- If the first player is eliminated during their own turn (e.g. self-damage power), the second player still takes their turn in step 3
- Resolution of the current action always completes (e.g. second hit of HIT_TWICE still fires)
- Winner is not determined until step 7, allowing burn to potentially eliminate the surviving player for a draw

---

## Unit Tests

---

### Zone.test.cs

```
ZONE-001
Description: Zone initializes with correct color and state
Input: Zone({ color: "red", state: "attack" })
Expected: zone.color === "red", zone.state === "attack"

ZONE-002
Description: Zone initializes with defend state
Input: Zone({ color: "blue", state: "defend" })
Expected: zone.color === "blue", zone.state === "defend"

ZONE-003
Description: Zone initializes with empty state
Input: Zone({ color: "green", state: "empty" })
Expected: zone.color === "green", zone.state === "empty"

ZONE-004
Description: Zone rejects invalid color
Input: Zone({ color: "purple", state: "attack" })
Expected: throws InvalidColorError

ZONE-005
Description: Zone rejects invalid state
Input: Zone({ color: "red", state: "explode" })
Expected: throws InvalidStateError
```

---

### Mana.test.cs

```
MANA-001
Description: Player starts with 10 empty mana slots
Input: new Player()
Expected: player.mana.length === 10, all slots have color "empty"

MANA-002
Description: Adding mana to an empty pool
Input: player with 10 empty slots, add "red" mana
Expected: one slot is now "red", 9 slots remain "empty", new red slot is the newest

MANA-003
Description: Adding mana when pool is full removes oldest
Input: player with 10 slots all filled [red(1), blue(2), green(3), yellow(4), red(5), blue(6), green(7), yellow(8), red(9), blue(10)] (numbers represent age, 1=oldest), add "yellow" mana
Expected: slot with red(1) is removed, yellow is added as newest. Pool is now [blue(2), green(3), yellow(4), red(5), blue(6), green(7), yellow(8), red(9), blue(10), yellow(new)]

MANA-004
Description: Colorless card generates no mana
Input: player plays colorless card, battle ends
Expected: player mana pool unchanged

MANA-005
Description: Spending colored mana consumes oldest of that color first
Input: player mana pool [red(1), blue(2), red(3), red(4)], spend 2 red mana
Expected: red(1) and red(3) consumed, pool is [blue(2), red(4)]

MANA-006
Description: Spending "any" mana consumes oldest overall
Input: player mana pool [red(1), blue(2), green(3)], spend 1 any mana
Expected: red(1) consumed, pool is [blue(2), green(3)]

MANA-007
Description: Spending more mana than available fails
Input: player mana pool [red(1), red(2)], attempt to spend 3 red mana
Expected: returns false, mana pool unchanged

MANA-008
Description: Mana requirement check passes when mana is present
Input: player mana pool [red(1), red(2), blue(3)], requirement: 2 red
Expected: returns true, mana pool unchanged

MANA-009
Description: Mana requirement check fails when mana is insufficient
Input: player mana pool [red(1), blue(2)], requirement: 2 red
Expected: returns false

MANA-010
Description: Mana requirement check for "any" passes with mixed colors
Input: player mana pool [red(1), blue(2), green(3)], requirement: 3 any
Expected: returns true, mana pool unchanged

MANA-011
Description: Mana persists between battles
Input: player ends battle 1 with [red(1), blue(2)], plays red card in battle 2
Expected: after battle 2 mana pool contains [red(1), blue(2), red(new)]
```

---

### Status.test.cs

```
STATUS-001
Description: Status initializes with correct properties
Input: Status({ type: "burn", duration: 3, power: 2 })
Expected: status.type === "burn", status.duration === 3, status.power === 2

STATUS-002
Description: Status duration decrements correctly
Input: Status({ type: "burn", duration: 2, power: 1 }), call decrement()
Expected: status.duration === 1

STATUS-003
Description: Status is removed when duration reaches 0
Input: Status({ type: "slow", duration: 1 }), call decrement()
Expected: status.duration === 0, status is flagged for removal

STATUS-004
Description: Multiple statuses of same type are tracked independently
Input: apply burn(power=1, duration=3) then burn(power=2, duration=1) to player
Expected: player.statuses.length === 2, first has power 1, second has power 2

STATUS-005
Description: Burn deals correct damage each turn
Input: player HP 20, burn(power=3, duration=2)
Expected: after burn resolves player HP === 17

STATUS-006
Description: Burn is not affected by protect
Input: player HP 20, protect(duration=1), burn(power=4, duration=1)
Expected: after burn resolves player HP === 16 (protect does not apply)

STATUS-007
Description: Burn is not affected by vulnerable
Input: player HP 20, vulnerable(duration=1), burn(power=4, duration=1)
Expected: after burn resolves player HP === 16 (vulnerable does not apply)

STATUS-008
Description: Renew heals correct amount each turn
Input: player HP 14, renew(power=3, duration=2)
Expected: after renew resolves player HP === 17

STATUS-009
Description: Renew cannot exceed maxHp
Input: player HP 19, maxHp 20, renew(power=3, duration=1)
Expected: after renew resolves player HP === 20

STATUS-010
Description: Renew does nothing when already at max HP
Input: player HP 20, maxHp 20, renew(power=3, duration=1)
Expected: after renew resolves player HP === 20

STATUS-011
Description: quick vs slow — quick goes first
Input: player1 has quick(duration=1), player2 has slow(duration=1), pre-assigned order is [player2, player1]
Expected: actingOrder === [player1, player2]

STATUS-012
Description: Both quick — default to pre-assigned order
Input: player1 has quick(duration=1), player2 has quick(duration=1), pre-assigned order is [player1, player2]
Expected: actingOrder === [player1, player2]

STATUS-013
Description: Both slow — default to pre-assigned order
Input: player1 has slow(duration=1), player2 has slow(duration=1), pre-assigned order is [player2, player1]
Expected: actingOrder === [player2, player1]

STATUS-014
Description: Two slows vs one quick — quick cancels one slow, other slow wins
Input: player1 has slow(duration=2) and slow(duration=1), player2 has quick(duration=1), pre-assigned order is [player1, player2]
Expected: actingOrder === [player1, player2] (player1 still effectively slow), both slow instances retain their durations

STATUS-015
Description: Silence prevents power activation
Input: player has silence(duration=1), card has a power with ON_HIT condition and a mana cost
Expected: power does not fire, mana is not spent or checked

STATUS-016
Description: Weakness(color) doubles damage from matching card color
Input: player2 has weakness(red)(duration=2), player1 plays red card with strength 4 hitting empty zone
Expected: damage dealt = 4 x 2 = 8

STATUS-017
Description: Weakness(color) does not apply to non-matching card color
Input: player2 has weakness(red)(duration=2), player1 plays blue card with strength 4 hitting empty zone
Expected: damage dealt = 4 (no multiplier)

STATUS-018
Description: Weakness(color) stacks with vulnerable
Input: player2 has weakness(red)(duration=1) and vulnerable(duration=1), player1 plays red card with strength 4 hitting empty zone
Expected: damage dealt = 4 x 2 x 2 = 16

STATUS-019
Description: Weakness(color) stacks with protect
Input: player2 has weakness(red)(duration=1) and protect(duration=1), player1 plays red card with strength 4 hitting empty zone
Expected: damage dealt = 4 x 2 x 0.5 = 4

STATUS-020
Description: Two weakness(red) stacks apply both multipliers
Input: player2 has weakness(red)(duration=1) and weakness(red)(duration=2), player1 plays red card with strength 4 hitting empty zone
Expected: damage dealt = 4 x 2 x 2 = 16

STATUS-021
Description: Weakness(color) does not apply to burn
Input: player2 has weakness(red)(duration=2) and burn(power=3, duration=1), burn was applied by a red card
Expected: burn deals 3 damage (weakness does not apply)

STATUS-022
Description: Statuses resolve in order of receipt
Input: player has burn(power=2, duration=1) received first, then burn(power=3, duration=1) received second, player HP 20
Expected: first burn resolves (HP=18), then second burn resolves (HP=15), order matches receipt order
```

---

### Battle.test.cs — Greatest Hit Rule

```
GHIT-001
Description: Single attack vs empty — HIT_EMPTY result
Input:
  Attacker zones: red=attack, blue=empty, green=empty, yellow=empty
  Defender zones: red=empty, blue=empty, green=empty, yellow=empty
Expected: result === "HIT_EMPTY"

GHIT-002
Description: Single attack vs attack — HIT_ATTACK result
Input:
  Attacker zones: red=attack, blue=empty, green=empty, yellow=empty
  Defender zones: red=attack, blue=empty, green=empty, yellow=empty
Expected: result === "HIT_ATTACK"

GHIT-003
Description: Single attack vs defend — NO_HIT result
Input:
  Attacker zones: red=attack, blue=empty, green=empty, yellow=empty
  Defender zones: red=defend, blue=empty, green=empty, yellow=empty
Expected: result === "NO_HIT"

GHIT-004
Description: Multiple attacks — one empty one attack — best result wins (HIT_EMPTY)
Input:
  Attacker zones: red=attack, blue=attack, green=empty, yellow=empty
  Defender zones: red=empty, blue=attack, green=empty, yellow=empty
Expected: result === "HIT_EMPTY"

GHIT-005
Description: Multiple attacks — one blocked one attack vs attack — HIT_ATTACK
Input:
  Attacker zones: red=attack, blue=attack, green=empty, yellow=empty
  Defender zones: red=defend, blue=attack, green=empty, yellow=empty
Expected: result === "HIT_ATTACK"

GHIT-006
Description: Multiple attacks — all blocked — NO_HIT
Input:
  Attacker zones: red=attack, blue=attack, green=attack, yellow=attack
  Defender zones: red=defend, blue=defend, green=defend, yellow=defend
Expected: result === "NO_HIT"

GHIT-007
Description: Multiple empty hits still count as one HIT_EMPTY
Input:
  Attacker zones: red=attack, blue=attack, green=empty, yellow=empty
  Defender zones: red=empty, blue=empty, green=empty, yellow=empty
Expected: result === "HIT_EMPTY", damage dealt equals one hit worth of damage, ON_HIT fires once

GHIT-008
Description: No attack zones — NO_HIT
Input:
  Attacker zones: red=defend, blue=defend, green=empty, yellow=empty
  Defender zones: red=empty, blue=empty, green=empty, yellow=empty
Expected: result === "NO_HIT"
```

---

### Battle.test.cs — Damage Calculation

```
DMG-001
Description: Full strength damage on HIT_EMPTY
Input: attacker strength=5, HIT_EMPTY, no modifiers
Expected: damage dealt = 5

DMG-002
Description: Half strength damage on HIT_ATTACK
Input: attacker strength=5, HIT_ATTACK, no modifiers
Expected: damage dealt = 2 (floor of 2.5)

DMG-003
Description: Weaken halves strength damage
Input: attacker strength=6, HIT_EMPTY, attacker has weaken
Expected: damage dealt = 3

DMG-004
Description: Weaken halves ability damage
Input: ability damage=6, HIT_EMPTY, attacker has weaken
Expected: damage dealt = 3

DMG-005
Description: Weaken does not apply to additional damage
Input: additional damage=4, attacker has weaken, defender has no modifiers
Expected: additional damage dealt = 4

DMG-006
Description: Protect halves strength damage
Input: attacker strength=6, HIT_EMPTY, defender has protect
Expected: damage dealt = 3

DMG-007
Description: Protect halves additional damage
Input: additional damage=4, defender has protect
Expected: additional damage dealt = 2

DMG-008
Description: Vulnerable doubles strength damage
Input: attacker strength=4, HIT_EMPTY, defender has vulnerable
Expected: damage dealt = 8

DMG-009
Description: Vulnerable doubles additional damage
Input: additional damage=3, defender has vulnerable
Expected: additional damage dealt = 6

DMG-010
Description: Weaken and vulnerable together on strength damage
Input: attacker strength=4, HIT_EMPTY, attacker has weaken, defender has vulnerable
Expected: damage dealt = 4 x 0.5 x 2 = 4

DMG-011
Description: Protect and vulnerable together cancel out
Input: attacker strength=4, HIT_EMPTY, defender has protect and vulnerable
Expected: damage dealt = 4 x 0.5 x 2 = 4

DMG-012
Description: Ability damage replaces strength on HIT_EMPTY
Input: card strength=5, ability damage power=3, HIT_EMPTY, no modifiers
Expected: damage dealt = 3 (not 5)

DMG-013
Description: Ability damage halved on HIT_ATTACK
Input: ability damage=6, HIT_ATTACK, no modifiers
Expected: damage dealt = 3

DMG-014
Description: Weaken does not apply to burn
Input: attacker has weaken, burn(power=4) resolves
Expected: burn damage = 4

DMG-015
Description: Protect does not apply to burn
Input: defender has protect, burn(power=4) resolves
Expected: burn damage = 4

DMG-016
Description: Vulnerable does not apply to burn
Input: defender has vulnerable, burn(power=4) resolves
Expected: burn damage = 4

DMG-017
Description: Weakness(red) applies to red card strength damage
Input: attacker is red card, strength=4, HIT_EMPTY, defender has weakness(red)
Expected: damage dealt = 8

DMG-018
Description: Weakness(red) applies to red card additional damage
Input: attacker is red card, additional damage=3, defender has weakness(red)
Expected: additional damage dealt = 6

DMG-019
Description: Weakness(red) does not apply to blue card damage
Input: attacker is blue card, strength=4, HIT_EMPTY, defender has weakness(red)
Expected: damage dealt = 4

DMG-020
Description: All modifiers together — red card, weaken, vulnerable, weakness(red), protect
Input: attacker is red card, strength=4, HIT_EMPTY, attacker has weaken, defender has vulnerable, weakness(red), protect
Expected: damage dealt = 4 x 0.5 x 2 x 2 x 0.5 = 4

DMG-021
Description: Ability damage and additional damage resolve in order
Input: card has ability damage=3 and additional damage=2 both ON_HIT, defender HP=20, HIT_EMPTY, no modifiers
Expected: defender HP after = 20 - 3 - 2 = 15, ability damage resolves first
```

---

### Battle.test.cs — Power Resolution

```
PWR-001
Description: ON_HIT power fires on HIT_EMPTY
Input: card has ON_HIT power (apply burn to opponent), result is HIT_EMPTY
Expected: opponent receives burn status

PWR-002
Description: ON_HIT power fires on HIT_ATTACK
Input: card has ON_HIT power (apply burn to opponent), result is HIT_ATTACK
Expected: opponent receives burn status

PWR-003
Description: ON_HIT power does not fire on NO_HIT
Input: card has ON_HIT power (apply burn to opponent), result is NO_HIT
Expected: opponent does not receive burn status

PWR-004
Description: ON_DEFEND power fires when defend successfully blocks
Input: card has red=defend, opponent has red=attack (ON_DEFEND: apply slow to opponent)
Expected: opponent receives slow status

PWR-005
Description: ON_DEFEND power does not fire when defend zone is not attacked
Input: card has red=defend, opponent has red=empty (ON_DEFEND: apply slow to opponent)
Expected: opponent does not receive slow status

PWR-006
Description: ON_BEING_HIT fires when this card's zone is hit
Input: this card has red=empty, opponent has red=attack (ON_BEING_HIT: apply vulnerable to opponent)
Expected: opponent receives vulnerable status

PWR-007
Description: ON_BEING_HIT fires when this card's zone is attack vs opponent attack
Input: this card has red=attack, opponent has red=attack (ON_BEING_HIT: apply vulnerable to opponent)
Expected: opponent receives vulnerable status

PWR-008
Description: ON_BEING_HIT does not fire when this card's zone blocks
Input: this card has red=defend, opponent has red=attack (ON_BEING_HIT: apply vulnerable to opponent)
Expected: opponent does not receive vulnerable status

PWR-009
Description: AFTER_TURN_RESOLVES fires after all zone resolution
Input: card has AFTER_TURN_RESOLVES power (deal 2 damage to both players), both players HP 20
Expected: both players HP = 18 after step 4

PWR-010
Description: Mana cost is spent when power activates
Input: player mana pool [blue(1), blue(2), red(3)], power costs 1 blue
Expected: blue(1) consumed, pool is [blue(2), red(3)]

PWR-011
Description: Power does not activate if mana cost not met
Input: player mana pool [red(1)], power costs 1 blue
Expected: power does not activate, mana pool unchanged

PWR-012
Description: Mana requirement is checked but not spent
Input: player mana pool [blue(1), blue(2)], power requires 2 blue
Expected: power activates, mana pool unchanged [blue(1), blue(2)]

PWR-013
Description: Power does not activate if mana requirement not met
Input: player mana pool [blue(1)], power requires 2 blue
Expected: power does not activate

PWR-014
Description: Power with both cost and requirement — both must be satisfied
Input: player mana pool [blue(1), blue(2), red(3)], power requires 2 blue present and costs 1 red
Expected: power activates, red(3) is consumed, pool is [blue(1), blue(2)]

PWR-015
Description: Power with both cost and requirement — requirement not met even if cost can be paid
Input: player mana pool [blue(1), red(2), red(3)], power requires 2 blue and costs 1 red
Expected: power does not activate, mana pool unchanged

PWR-016
Description: HIT_TWICE causes ON_HIT to fire twice
Input: card has HIT_TWICE power and ON_HIT apply_status power, result is HIT_EMPTY
Expected: status is applied twice (two independent instances)

PWR-017
Description: HIT_TWICE — both hits resolve on HIT_ATTACK (both halved)
Input: card strength=4, HIT_TWICE active, result is HIT_ATTACK, no modifiers
Expected: total damage = 2 + 2 = 4 (each hit halved independently)

PWR-018
Description: HIT_TWICE — both hits resolve on HIT_EMPTY (both full)
Input: card strength=4, HIT_TWICE active, result is HIT_EMPTY, no modifiers
Expected: total damage = 4 + 4 = 8

PWR-019
Description: HIT_TWICE — first hit eliminates opponent, second hit still resolves
Input: card strength=4, HIT_TWICE, HIT_EMPTY, opponent HP=3, no modifiers
Expected: first hit reduces opponent HP to -1 (eliminated), second hit still resolves reducing HP further, winner determined at step 7

PWR-020
Description: Silence prevents all powers from firing
Input: player has silence(duration=1), card has ON_HIT power with mana cost
Expected: ON_HIT power does not fire, mana not spent, no effect applied
```

---

### Battle.test.cs — Full Battle Resolution

```
BATTLE-001
Description: Basic full battle — attacker hits empty, no powers, no modifiers
Input:
  player1 HP=20, card strength=4, zones: red=attack, blue=empty, green=empty, yellow=empty
  player2 HP=20, card strength=3, zones: red=empty, blue=empty, green=empty, yellow=empty
  turn order: [player1, player2]
Expected:
  After step 2: player2 HP = 16
  After step 3: player1 HP unchanged (player2 has no attack zones)
  After step 7: no winner

BATTLE-002
Description: Player eliminated during first player's turn — second player does not act
Input:
  player1 HP=20, card strength=20, zones: red=attack
  player2 HP=15, card strength=5, zones: red=attack
  Defender red=empty guaranteed HIT_EMPTY for player1
  turn order: [player1, player2]
Expected:
  After step 2: player2 HP = 0, isEliminated = true
  Step 3 is skipped
  After step 7: result === "player1"

BATTLE-003
Description: Player1 eliminated during their own turn by self-damage — second player still acts
Input:
  player1 HP=2, card has AFTER_TURN_RESOLVES power dealing 3 damage to both players
  player2 HP=20, card strength=4, zones: red=attack, player1 red=empty
  turn order: [player1, player2]
Expected:
  Step 3: player2 takes their attack turn
  After step 4: player1 HP = 2 - 3 = -1 (eliminated), player2 HP = 20 - 3 = 17
  After step 7: result === "player2"

BATTLE-004
Description: Draw — both players eliminated by simultaneous self-damage power
Input:
  player1 HP=3, player2 HP=3
  player1 card has AFTER_TURN_RESOLVES power dealing 5 damage to both players
  neither player has attack zones
  turn order: [player1, player2]
Expected:
  After step 4: player1 HP = -2, player2 HP = -2, both isEliminated
  After step 7: result === "draw"

BATTLE-005
Description: Draw — player1 wins attack but dies to burn
Input:
  player1 HP=20, card strength=20, zones: red=attack, player2 red=empty
  player1 has burn(power=20, duration=1)
  player2 HP=10, no attack zones
  turn order: [player1, player2]
Expected:
  After step 2: player2 HP = 0, isEliminated
  Step 3: skipped
  After step 5: player1 burn deals 20, player1 HP = 0, isEliminated
  After step 7: result === "draw"

BATTLE-006
Description: Mana generated after battle for colored card
Input: player plays red card, player mana has 5 empty slots
Expected: after step 8, player gains 1 red mana, pool has 4 empty slots and 1 red slot

BATTLE-007
Description: No mana generated for colorless card
Input: player plays colorless card
Expected: after step 8, player mana pool unchanged

BATTLE-008
Description: Full battle with ON_HIT burn application and burn tick
Input:
  player1 HP=20, red card strength=3, zones: red=attack
  player1 ON_HIT power: apply burn(power=2, duration=2) to player2, no mana cost
  player2 HP=20, zones: red=empty
  turn order: [player1, player2]
Expected:
  After step 2: player2 HP = 17, player2 has burn(power=2, duration=2)
  After step 5: player2 HP = 15 (burn ticks for 2)
  After step 6: burn duration decremented to 1
  After step 7: no winner
  Battle 2 step 5: player2 HP = 13, burn duration decremented to 0 and removed

BATTLE-009
Description: ON_DEFEND applies status to attacker
Input:
  player1 card: red=defend, ON_DEFEND power: apply weaken(duration=2) to opponent
  player2 card: red=attack
  turn order: [player2, player1]
Expected:
  During player2 attack resolution: player1 defends red, ON_DEFEND fires
  player2 receives weaken(duration=2)

BATTLE-010
Description: HIT_TWICE with ON_HIT status — status applied twice
Input:
  player1 card has HIT_TWICE power (costs 2 blue mana) and ON_HIT apply vulnerable(duration=1) power
  player1 mana: [blue(1), blue(2), blue(3)]
  player1 zones: red=attack, player2 red=empty (HIT_EMPTY)
Expected:
  player2 receives vulnerable(duration=1) twice (two independent stacks)
  player1 mana after: [blue(3)] (blue(1) and blue(2) consumed)

BATTLE-011
Description: Statuses tick and are removed correctly over multiple battles
Input:
  player has slow(duration=2) applied at start of battle 1
Expected:
  Battle 1 step 6: slow duration decremented to 1
  Battle 2 step 6: slow duration decremented to 0, slow removed
  Battle 3: no slow present on player

BATTLE-012
Description: Player eliminated mid-turn does not receive renew healing
Input:
  player2 HP=2, has renew(power=5, duration=2)
  player1 card strength=5, zones: red=attack, player2 red=empty (HIT_EMPTY)
  turn order: [player1, player2]
Expected:
  After step 2: player2 HP = -3, isEliminated = true
  Step 5: player2 renew does not fire (player2 is eliminated)
  player2 HP remains at -3, renew healing is not applied
```

---

## Implementation Notes

- All damage calculations that result in non-integer values should use **floor** rounding
- All status and power resolution should be processed in **order of receipt** using the `receivedAt` timestamp or index
- The `isEliminated` flag should be checked after every damage-dealing operation, not just at the end of a step
- `weakness(color)` should store the color as part of the status instance (e.g. `{ type: "weakness", color: "red", duration: 2 }`)
- Mana age should be tracked by insertion order — a simple array where index 0 is oldest is sufficient
- Powers should be evaluated and resolved sequentially, not simultaneously, unless otherwise specified
- When both a mana cost and mana requirement exist on the same power, check the requirement first, then check and spend the cost. If either fails, the power does not activate and no mana is spent
