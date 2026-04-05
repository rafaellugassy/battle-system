namespace BattleSystem.Models;

/// <summary>
/// Represents a card fielded by a player in a battle.
/// All properties are fixed and defined at construction.
/// </summary>
public class Card
{
    private static readonly HashSet<string> ValidColors = new() { "red", "blue", "yellow", "green", "colorless" };

    public string Id { get; }
    public string Name { get; }
    public string Color { get; }
    public int Strength { get; }
    public Zone[] Zones { get; }
    public List<Power> Powers { get; }

    public Card(string id, string name, string color, int strength, Zone[] zones, List<Power> powers)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Card ID cannot be null or empty", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Card name cannot be null or empty", nameof(name));
        }

        if (!ValidColors.Contains(color))
        {
            throw new ArgumentException($"Invalid color: {color}. Must be one of: red, blue, yellow, green, colorless", nameof(color));
        }

        if (strength < 0)
        {
            throw new ArgumentException("Strength cannot be negative", nameof(strength));
        }

        if (zones == null || zones.Length != 4)
        {
            throw new ArgumentException("Card must have exactly 4 zones", nameof(zones));
        }

        // Validate that there's exactly one zone per color
        var zoneColors = zones.Select(z => z.Color).ToHashSet();
        var requiredColors = new HashSet<string> { "red", "blue", "yellow", "green" };
        if (!zoneColors.SetEquals(requiredColors))
        {
            throw new ArgumentException("Card must have exactly one zone per color (red, blue, yellow, green)", nameof(zones));
        }

        if (powers == null || powers.Count == 0)
        {
            throw new ArgumentException("Card must have at least one power", nameof(powers));
        }

        // Validate only one ABILITY_DAMAGE power
        var abilityDamageCount = powers.Count(p => p.Effect.Type == EffectType.ABILITY_DAMAGE);
        if (abilityDamageCount > 1)
        {
            throw new ArgumentException("Card can only have one ABILITY_DAMAGE power", nameof(powers));
        }

        Id = id;
        Name = name;
        Color = color;
        Strength = strength;
        Zones = zones;
        Powers = powers;
    }

    public bool IsColorless => Color == "colorless";

    public Zone GetZone(string color)
    {
        return Zones.First(z => z.Color == color);
    }
}

// Made with Bob
