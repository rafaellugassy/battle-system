namespace BattleSystem.Models;

/// <summary>
/// Defines when a power activates during battle resolution.
/// </summary>
public enum ConditionType
{
    /// <summary>
    /// Activates when this card's attack resolves as a hit per the Greatest Hit Rule.
    /// Fires once per attack resolution, twice if HIT_TWICE is active.
    /// </summary>
    ON_HIT,

    /// <summary>
    /// Activates when this card's defend zone successfully nullifies an incoming attack.
    /// Fires independently for each defend zone that blocks.
    /// </summary>
    ON_DEFEND,

    /// <summary>
    /// Activates when an opponent's attack zone hits this card.
    /// </summary>
    ON_BEING_HIT,

    /// <summary>
    /// Activates after all attack and defend resolution for the turn is complete.
    /// </summary>
    AFTER_TURN_RESOLVES
}

public class Condition
{
    public ConditionType Type { get; }

    public Condition(ConditionType type)
    {
        Type = type;
    }
}

// Made with Bob
