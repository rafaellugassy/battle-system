namespace BattleSystem.Models;

/// <summary>
/// Represents a single mana slot for a player.
/// Tracks color and age (insertion order) for spending priority.
/// </summary>
public class Mana
{
    private static readonly HashSet<string> ValidColors = new() { "empty", "red", "blue", "yellow", "green" };

    public string Color { get; set; }
    public int ReceivedAt { get; set; }

    public Mana(string color, int receivedAt)
    {
        if (!ValidColors.Contains(color))
        {
            throw new ArgumentException($"Invalid color: {color}. Must be one of: empty, red, blue, yellow, green", nameof(color));
        }

        Color = color;
        ReceivedAt = receivedAt;
    }

    public bool IsEmpty => Color == "empty";
}

// Made with Bob
