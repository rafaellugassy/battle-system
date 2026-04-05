namespace BattleSystem.Models;

/// <summary>
/// Defines what a power does when activated.
/// </summary>
public enum EffectType
{
    /// <summary>
    /// Replaces the card's strength damage. Only one per card.
    /// </summary>
    ABILITY_DAMAGE,

    /// <summary>
    /// Deals damage after the main attack resolves.
    /// </summary>
    ADDITIONAL_DAMAGE,

    /// <summary>
    /// Applies a status to a target player.
    /// </summary>
    APPLY_STATUS,

    /// <summary>
    /// Heals HP on a target player.
    /// </summary>
    HEAL,

    /// <summary>
    /// Causes the card's attack to hit twice.
    /// </summary>
    HIT_TWICE
}

public enum EffectTarget
{
    SELF,
    OPPONENT,
    BOTH
}

public class Effect
{
    public EffectType Type { get; }
    public EffectTarget Target { get; }
    public int? Value { get; }
    public Status? StatusToApply { get; }

    public Effect(EffectType type, EffectTarget target, int? value = null, Status? statusToApply = null)
    {
        Type = type;
        Target = target;
        Value = value;
        StatusToApply = statusToApply;

        // Validation
        if ((type == EffectType.ABILITY_DAMAGE || type == EffectType.ADDITIONAL_DAMAGE || type == EffectType.HEAL) && !value.HasValue)
        {
            throw new ArgumentException($"Effect type {type} requires a value", nameof(value));
        }

        if (type == EffectType.APPLY_STATUS && statusToApply == null)
        {
            throw new ArgumentException("APPLY_STATUS effect requires a status to apply", nameof(statusToApply));
        }
    }
}

// Made with Bob
