namespace BattleSystem.Models;

/// <summary>
/// Defines mana that is consumed when a power activates.
/// </summary>
public class CostEntry
{
    private static readonly HashSet<string> ValidColors = new() { "red", "blue", "yellow", "green", "any" };

    public string Color { get; }
    public int Amount { get; }

    public CostEntry(string color, int amount)
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

public class ManaCost
{
    public List<CostEntry> Costs { get; }

    public ManaCost(List<CostEntry> costs)
    {
        Costs = costs ?? throw new ArgumentNullException(nameof(costs));
    }

    public ManaCost(string color, int amount)
    {
        Costs = new List<CostEntry> { new CostEntry(color, amount) };
    }
}

// Made with Bob
