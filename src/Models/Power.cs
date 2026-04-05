namespace BattleSystem.Models;

/// <summary>
/// Represents an ability on a card.
/// Each power has a condition that determines when it fires, optional mana interactions, and an effect.
/// </summary>
public class Power
{
    public string Id { get; }
    public Condition Condition { get; }
    public Effect Effect { get; }
    public ManaCost? ManaCost { get; }
    public ManaRequirement? ManaRequirement { get; }

    public Power(string id, Condition condition, Effect effect, ManaCost? manaCost = null, ManaRequirement? manaRequirement = null)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Power ID cannot be null or empty", nameof(id));
        }

        Id = id;
        Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        Effect = effect ?? throw new ArgumentNullException(nameof(effect));
        ManaCost = manaCost;
        ManaRequirement = manaRequirement;
    }
}

// Made with Bob
