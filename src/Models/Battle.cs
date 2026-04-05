namespace BattleSystem.Models;

/// <summary>
/// Manages the full resolution of a single battle between two players.
/// Implements all 8 steps of battle resolution according to the specification.
/// </summary>
public class Battle
{
    public Player Player1 { get; }
    public Player Player2 { get; }
    public Player[] TurnOrder { get; }
    public string? Result { get; private set; }

    private enum HitResult
    {
        HIT_EMPTY,
        HIT_ATTACK,
        NO_HIT
    }

    public Battle(Player player1, Player player2, Player[] turnOrder)
    {
        Player1 = player1 ?? throw new ArgumentNullException(nameof(player1));
        Player2 = player2 ?? throw new ArgumentNullException(nameof(player2));
        TurnOrder = turnOrder ?? throw new ArgumentNullException(nameof(turnOrder));

        if (turnOrder.Length != 2)
        {
            throw new ArgumentException("Turn order must contain exactly 2 players", nameof(turnOrder));
        }

        Result = null;
    }

    /// <summary>
    /// Executes the full battle resolution.
    /// </summary>
    public void Resolve()
    {
        // Step 1: Determine Acting Order
        var actingOrder = DetermineActingOrder();

        // Step 2: First Player Attack Resolution
        ResolvePlayerAttack(actingOrder[0], actingOrder[1]);

        // Step 3: Second Player Attack Resolution
        if (!actingOrder[1].IsEliminated)
        {
            ResolvePlayerAttack(actingOrder[1], actingOrder[0]);
        }

        // Step 4: After Turn Resolves
        ResolveAfterTurnPowers();

        // Step 5: Burn and Renew
        ResolveBurnAndRenew();

        // Step 6: Status Duration Decrement
        DecrementStatusDurations();

        // Step 7: Determine Winner
        DetermineWinner();

        // Step 8: Mana Generation
        GenerateMana();
    }

    /// <summary>
    /// Step 1: Determine acting order based on quick/slow statuses.
    /// </summary>
    public Player[] DetermineActingOrder()
    {
        var p1QuickCount = Player1.GetStatusCount("quick");
        var p1SlowCount = Player1.GetStatusCount("slow");
        var p2QuickCount = Player2.GetStatusCount("quick");
        var p2SlowCount = Player2.GetStatusCount("slow");

        var p1NetSpeed = p1QuickCount - p1SlowCount;
        var p2NetSpeed = p2QuickCount - p2SlowCount;

        // If one player is faster, they go first
        if (p1NetSpeed > p2NetSpeed)
        {
            return new[] { Player1, Player2 };
        }
        else if (p2NetSpeed > p1NetSpeed)
        {
            return new[] { Player2, Player1 };
        }

        // Otherwise use pre-assigned turn order
        return TurnOrder;
    }

    /// <summary>
    /// Steps 2 & 3: Resolve a player's attack against their opponent.
    /// </summary>
    private void ResolvePlayerAttack(Player attacker, Player defender)
    {
        if (attacker.IsEliminated) return;

        // Apply Greatest Hit Rule
        var hitResult = EvaluateGreatestHit(attacker.Card, defender.Card);

        // Calculate and apply damage
        if (hitResult != HitResult.NO_HIT)
        {
            var baseDamage = CalculateBaseDamage(attacker.Card, hitResult);
            var finalDamage = ApplyDamageModifiers(baseDamage, attacker, defender);
            
            defender.TakeDamage(finalDamage);

            // Check for HIT_TWICE power
            var hitTwice = attacker.Card.Powers.Any(p => 
                p.Effect.Type == EffectType.HIT_TWICE && 
                p.Condition.Type == ConditionType.ON_HIT &&
                CanActivatePower(attacker, p));

            // Fire ON_HIT powers
            FirePowersForCondition(attacker, defender, ConditionType.ON_HIT);

            // If HIT_TWICE, apply damage and fire powers again
            if (hitTwice)
            {
                defender.TakeDamage(finalDamage);
                FirePowersForCondition(attacker, defender, ConditionType.ON_HIT);
            }
        }

        // Fire ON_DEFEND powers for each successful defend
        FireOnDefendPowers(attacker, defender);

        // Fire ON_BEING_HIT powers for each zone that was hit
        FireOnBeingHitPowers(attacker, defender);
    }

    /// <summary>
    /// Evaluates all zones and returns the best hit result (Greatest Hit Rule).
    /// </summary>
    private HitResult EvaluateGreatestHit(Card attackerCard, Card defenderCard)
    {
        var hasHitEmpty = false;
        var hasHitAttack = false;

        foreach (var attackerZone in attackerCard.Zones)
        {
            if (attackerZone.State != "attack") continue;

            var defenderZone = defenderCard.GetZone(attackerZone.Color);

            if (defenderZone.State == "empty")
            {
                hasHitEmpty = true;
            }
            else if (defenderZone.State == "attack")
            {
                hasHitAttack = true;
            }
        }

        if (hasHitEmpty) return HitResult.HIT_EMPTY;
        if (hasHitAttack) return HitResult.HIT_ATTACK;
        return HitResult.NO_HIT;
    }

    /// <summary>
    /// Calculates base damage before modifiers.
    /// </summary>
    private int CalculateBaseDamage(Card card, HitResult hitResult)
    {
        // Check for ABILITY_DAMAGE power
        var abilityDamagePower = card.Powers.FirstOrDefault(p => p.Effect.Type == EffectType.ABILITY_DAMAGE);
        var baseDamage = abilityDamagePower?.Effect.Value ?? card.Strength;

        // HIT_ATTACK deals half damage (floor rounding)
        if (hitResult == HitResult.HIT_ATTACK)
        {
            baseDamage = (int)Math.Floor(baseDamage / 2.0);
        }

        return baseDamage;
    }

    /// <summary>
    /// Applies damage modifiers (weaken, protect, vulnerable, weakness).
    /// </summary>
    private int ApplyDamageModifiers(int baseDamage, Player attacker, Player defender, bool applyWeaken = true)
    {
        var multiplier = 1.0;

        // Weaken on attacker (x0.5) - only applies to strength/ability damage, not additional damage
        if (applyWeaken && attacker.HasStatus("weaken"))
        {
            multiplier *= 0.5;
        }

        // Protect on defender (x0.5)
        if (defender.HasStatus("protect"))
        {
            multiplier *= 0.5;
        }

        // Vulnerable on defender (x2)
        if (defender.HasStatus("vulnerable"))
        {
            multiplier *= 2.0;
        }

        // Weakness(color) on defender (x2 per matching weakness)
        var weaknessStatuses = defender.Statuses.Where(s =>
            s.Type.StartsWith("weakness(") &&
            s.WeaknessColor == attacker.Card.Color);
        
        foreach (var _ in weaknessStatuses)
        {
            multiplier *= 2.0;
        }

        return (int)Math.Floor(baseDamage * multiplier);
    }

    /// <summary>
    /// Fires powers that match the given condition.
    /// </summary>
    private void FirePowersForCondition(Player owner, Player opponent, ConditionType conditionType)
    {
        if (owner.IsEliminated || owner.IsSilenced) return;

        var powers = owner.Card.Powers.Where(p => p.Condition.Type == conditionType).ToList();

        foreach (var power in powers)
        {
            if (CanActivatePower(owner, power))
            {
                ActivatePower(owner, opponent, power);
            }
        }
    }

    /// <summary>
    /// Fires ON_DEFEND powers if any defend zone blocked.
    /// </summary>
    private void FireOnDefendPowers(Player attacker, Player defender)
    {
        if (defender.IsEliminated || defender.IsSilenced) return;

        // Check if any defend zone blocked an attack
        var hasDefendBlock = false;
        foreach (var defenderZone in defender.Card.Zones)
        {
            if (defenderZone.State != "defend") continue;

            var attackerZone = attacker.Card.GetZone(defenderZone.Color);
            if (attackerZone.State == "attack")
            {
                hasDefendBlock = true;
                break;
            }
        }

        // Fire ON_DEFEND powers once if any defend blocked
        if (hasDefendBlock)
        {
            FirePowersForCondition(defender, attacker, ConditionType.ON_DEFEND);
        }
    }

    /// <summary>
    /// Fires ON_BEING_HIT powers if any zone was hit.
    /// </summary>
    private void FireOnBeingHitPowers(Player attacker, Player defender)
    {
        if (defender.IsEliminated || defender.IsSilenced) return;

        // Check if any zone was hit
        var wasHit = false;
        foreach (var defenderZone in defender.Card.Zones)
        {
            var attackerZone = attacker.Card.GetZone(defenderZone.Color);
            
            // Zone is hit if it's empty or attack, and opponent zone is attack
            if ((defenderZone.State == "empty" || defenderZone.State == "attack") &&
                attackerZone.State == "attack")
            {
                wasHit = true;
                break;
            }
        }

        // Fire ON_BEING_HIT powers once if any zone was hit
        if (wasHit)
        {
            FirePowersForCondition(defender, attacker, ConditionType.ON_BEING_HIT);
        }
    }

    /// <summary>
    /// Checks if a power can activate (mana requirements and costs).
    /// </summary>
    private bool CanActivatePower(Player owner, Power power)
    {
        if (owner.IsSilenced) return false;

        // Check mana requirement first
        if (power.ManaRequirement != null && !owner.CanMeetRequirement(power.ManaRequirement))
        {
            return false;
        }

        // Check mana cost
        if (power.ManaCost != null)
        {
            // We need to check without spending first
            var canAfford = true;
            foreach (var cost in power.ManaCost.Costs)
            {
                var available = cost.Color == "any" 
                    ? owner.ManaPool.Count(m => !m.IsEmpty)
                    : owner.ManaPool.Count(m => m.Color == cost.Color);
                
                if (available < cost.Amount)
                {
                    canAfford = false;
                    break;
                }
            }
            return canAfford;
        }

        return true;
    }

    /// <summary>
    /// Activates a power's effect.
    /// </summary>
    private void ActivatePower(Player owner, Player opponent, Power power)
    {
        // Spend mana cost if present
        if (power.ManaCost != null)
        {
            owner.TrySpendMana(power.ManaCost);
        }

        // Apply effect based on target
        var targets = power.Effect.Target switch
        {
            EffectTarget.SELF => new[] { owner },
            EffectTarget.OPPONENT => new[] { opponent },
            EffectTarget.BOTH => new[] { owner, opponent },
            _ => Array.Empty<Player>()
        };

        foreach (var target in targets)
        {
            ApplyEffect(power.Effect, target, owner);
        }
    }

    /// <summary>
    /// Applies an effect to a target player.
    /// </summary>
    private void ApplyEffect(Effect effect, Player target, Player source)
    {
        if (target.IsEliminated && effect.Type != EffectType.ADDITIONAL_DAMAGE) return;

        switch (effect.Type)
        {
            case EffectType.ADDITIONAL_DAMAGE:
                if (!target.IsEliminated && effect.Value.HasValue)
                {
                    // Additional damage does not get weaken modifier
                    var damage = ApplyDamageModifiers(effect.Value.Value, source, target, applyWeaken: false);
                    target.TakeDamage(damage);
                }
                break;

            case EffectType.HEAL:
                if (effect.Value.HasValue)
                {
                    target.Heal(effect.Value.Value);
                }
                break;

            case EffectType.APPLY_STATUS:
                if (effect.StatusToApply != null)
                {
                    // Create a new status instance to avoid sharing the same object
                    var newStatus = new Status(
                        effect.StatusToApply.Type,
                        effect.StatusToApply.Duration,
                        effect.StatusToApply.Power,
                        effect.StatusToApply.ReceivedAt,
                        effect.StatusToApply.WeaknessColor
                    );
                    target.AddStatus(newStatus);
                }
                break;

            case EffectType.HIT_TWICE:
            case EffectType.ABILITY_DAMAGE:
                // These are handled elsewhere
                break;
        }
    }

    /// <summary>
    /// Step 4: Resolve AFTER_TURN_RESOLVES powers.
    /// </summary>
    private void ResolveAfterTurnPowers()
    {
        var players = new[] { Player1, Player2 };

        foreach (var player in players)
        {
            if (player.IsEliminated) continue;

            var opponent = player == Player1 ? Player2 : Player1;
            FirePowersForCondition(player, opponent, ConditionType.AFTER_TURN_RESOLVES);
        }
    }

    /// <summary>
    /// Step 5: Resolve burn and renew statuses.
    /// </summary>
    private void ResolveBurnAndRenew()
    {
        var players = new[] { Player1, Player2 };

        foreach (var player in players)
        {
            if (player.IsEliminated) continue;

            // Process statuses in order of receipt
            foreach (var status in player.Statuses.OrderBy(s => s.ReceivedAt))
            {
                if (status.Type == "burn" && status.Power.HasValue)
                {
                    player.TakeDamage(status.Power.Value);
                }
                else if (status.Type == "renew" && status.Power.HasValue)
                {
                    player.Heal(status.Power.Value);
                }
            }
        }
    }

    /// <summary>
    /// Step 6: Decrement status durations and remove expired ones.
    /// PUBLIC for game loop - call this between battles.
    /// </summary>
    public void DecrementStatusDurations()
    {
        if (!Player1.IsEliminated) Player1.TickStatuses();
        if (!Player2.IsEliminated) Player2.TickStatuses();
    }

    /// <summary>
    /// Step 7: Determine the winner.
    /// </summary>
    private void DetermineWinner()
    {
        if (Player1.IsEliminated && Player2.IsEliminated)
        {
            Result = "draw";
        }
        else if (Player1.IsEliminated)
        {
            Result = "player2";
        }
        else if (Player2.IsEliminated)
        {
            Result = "player1";
        }
        // else Result remains null (no winner yet)
    }

    /// <summary>
    /// Step 8: Generate mana for both players.
    /// PUBLIC for game loop - call this between battles.
    /// </summary>
    public void GenerateMana()
    {
        if (!Player1.Card.IsColorless)
        {
            Player1.AddMana(Player1.Card.Color);
        }

        if (!Player2.Card.IsColorless)
        {
            Player2.AddMana(Player2.Card.Color);
        }
    }
}

// Made with Bob
