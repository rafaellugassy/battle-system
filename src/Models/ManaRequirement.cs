namespace BattleSystem.Models;

/// <summary>
/// Defines mana that must be present for a power to activate but is not consumed.
/// </summary>
public class RequirementEntry
{
    private static readonly HashSet<string> ValidColors = new() { "red", "blue", "yellow", "green", "any" };

    public string Color { get; }
    public int Amount { get; }

    public RequirementEntry(string color, int amount)
    {
        if (!ValidColors.Contains(color))
        {
            throw new ArgumentException($"Invalid color: {color}. Must be one of: red, blue, yellow, green, any", nameof(color));
        }

        if (amount < 0)
        {
            throw new ArgumentException("Amount cannot be negative", nameof(amount));
        }

        Color = color;
        Amount = amount;
    }
}

public class ManaRequirement
{
    public List<RequirementEntry> Requirements { get; }

    public ManaRequirement(List<RequirementEntry> requirements)
    {
        Requirements = requirements ?? throw new ArgumentNullException(nameof(requirements));
    }

    public ManaRequirement(string color, int amount)
    {
        Requirements = new List<RequirementEntry> { new RequirementEntry(color, amount) };
    }
}

// Made with Bob
