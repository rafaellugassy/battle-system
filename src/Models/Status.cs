namespace BattleSystem.Models;

/// <summary>
/// Represents a status effect applied to a player.
/// Each instance is independent even if the same type is applied multiple times.
/// </summary>
public class Status
{
    private static readonly HashSet<string> ValidTypes = new() 
    { 
        "slow", "quick", "silence", "protect", "weaken", 
        "vulnerable", "weakness", "burn", "renew" 
    };

    public string Type { get; }
    public int Duration { get; set; }
    public int? Power { get; }
    public int ReceivedAt { get; }
    public string? WeaknessColor { get; }

    public Status(string type, int duration, int? power = null, int receivedAt = 0, string? weaknessColor = null)
    {
        if (!ValidTypes.Contains(type))
        {
            throw new ArgumentException($"Invalid status type: {type}", nameof(type));
        }

        if (duration < 0)
        {
            throw new ArgumentException("Duration cannot be negative", nameof(duration));
        }

        // Validate weakness color if type is weakness
        if (type == "weakness" && string.IsNullOrEmpty(weaknessColor))
        {
            throw new ArgumentException("Weakness status requires a color", nameof(weaknessColor));
        }

        Type = type;
        Duration = duration;
        Power = power;
        ReceivedAt = receivedAt;
        WeaknessColor = weaknessColor;
    }

    public void DecrementDuration()
    {
        if (Duration > 0)
        {
            Duration--;
        }
    }

    public bool ShouldRemove => Duration <= 0;
}

// Made with Bob
