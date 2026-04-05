namespace BattleSystem.Models;

/// <summary>
/// Represents one of four colored slots on a card.
/// Each zone has a fixed color and a fixed state.
/// </summary>
public class Zone
{
    private static readonly HashSet<string> ValidColors = new() { "red", "blue", "yellow", "green" };
    private static readonly HashSet<string> ValidStates = new() { "attack", "defend", "empty" };

    public string Color { get; }
    public string State { get; }

    public Zone(string color, string state)
    {
        if (!ValidColors.Contains(color))
        {
            throw new ArgumentException($"Invalid color: {color}. Must be one of: red, blue, yellow, green", nameof(color));
        }

        if (!ValidStates.Contains(state))
        {
            throw new ArgumentException($"Invalid state: {state}. Must be one of: attack, defend, empty", nameof(state));
        }

        Color = color;
        State = state;
    }
}

// Made with Bob
