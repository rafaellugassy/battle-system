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
        // Extract base type for weakness(color) format
        var baseType = type;
        if (type.StartsWith("weakness(") && type.EndsWith(")"))
        {
            baseType = "weakness";
            // Extract color from weakness(color) format if not provided separately
            if (string.IsNullOrEmpty(weaknessColor))
            {
                weaknessColor = type.Substring(9, type.Length - 10); // Extract color between ( and )
            }
        }

        if (!ValidTypes.Contains(baseType))
        {
            throw new ArgumentException($"Invalid status type: {type}", nameof(type));
        }

        if (duration < 0)
        {
            throw new ArgumentException("Duration cannot be negative", nameof(duration));
        }

        // Validate weakness color if type is weakness
        if (baseType == "weakness" && string.IsNullOrEmpty(weaknessColor))
        {
            throw new ArgumentException("Weakness status requires a color", nameof(weaknessColor));
        }

        Type = type; // Keep full type including weakness(color) format
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
