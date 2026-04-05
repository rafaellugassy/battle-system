namespace BattleSystem.Models;

/// <summary>
/// Represents a player participating in a battle.
/// Manages HP, mana, statuses, and the fielded card.
/// </summary>
public class Player
{
    private const int MaxManaSlots = 10;
    private const int DefaultMaxHp = 20;
    private int _battleIndex = 0;

    public string Id { get; }
    public int Hp { get; set; }
    public int MaxHp { get; set; }
    public Mana[] ManaPool { get; }
    public List<Status> Statuses { get; }
    public Card Card { get; set; }
    public bool IsEliminated { get; set; }

    public Player(string id, Card card, int hp = DefaultMaxHp)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Player ID cannot be null or empty", nameof(id));
        }

        Id = id;
        Hp = hp;
        MaxHp = hp;
        Card = card ?? throw new ArgumentNullException(nameof(card));
        IsEliminated = false;

        // Initialize mana pool with 10 empty slots
        ManaPool = new Mana[MaxManaSlots];
        for (int i = 0; i < MaxManaSlots; i++)
        {
            ManaPool[i] = new Mana("empty", 0);
        }

        Statuses = new List<Status>();
    }

    /// <summary>
    /// Adds mana to the player's pool. If full, removes oldest mana first.
    /// </summary>
    public void AddMana(string color)
    {
        if (color == "empty" || color == "colorless")
        {
            return; // Don't add empty or colorless mana
        }

        _battleIndex++;

        // Find first empty slot
        var emptySlot = Array.FindIndex(ManaPool, m => m.IsEmpty);
        
        if (emptySlot >= 0)
        {
            // Add to empty slot
            ManaPool[emptySlot] = new Mana(color, _battleIndex);
        }
        else
        {
            // Pool is full, remove oldest and shift
            for (int i = 0; i < MaxManaSlots - 1; i++)
            {
                ManaPool[i] = ManaPool[i + 1];
            }
            ManaPool[MaxManaSlots - 1] = new Mana(color, _battleIndex);
        }
    }

    /// <summary>
    /// Checks if player can meet a mana requirement (without spending).
    /// </summary>
    public bool CanMeetRequirement(ManaRequirement requirement)
    {
        foreach (var req in requirement.Requirements)
        {
            if (req.Color == "any")
            {
                var nonEmptyCount = ManaPool.Count(m => !m.IsEmpty);
                if (nonEmptyCount < req.Amount)
                {
                    return false;
                }
            }
            else
            {
                var colorCount = ManaPool.Count(m => m.Color == req.Color);
                if (colorCount < req.Amount)
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Attempts to spend mana for a cost. Returns true if successful.
    /// </summary>
    public bool TrySpendMana(ManaCost cost)
    {
        // First check if we can afford it
        foreach (var costEntry in cost.Costs)
        {
            if (costEntry.Color == "any")
            {
                var nonEmptyCount = ManaPool.Count(m => !m.IsEmpty);
                if (nonEmptyCount < costEntry.Amount)
                {
                    return false;
                }
            }
            else
            {
                var colorCount = ManaPool.Count(m => m.Color == costEntry.Color);
                if (colorCount < costEntry.Amount)
                {
                    return false;
                }
            }
        }

        // We can afford it, now spend it
        foreach (var costEntry in cost.Costs)
        {
            SpendManaOfColor(costEntry.Color, costEntry.Amount);
        }

        return true;
    }

    private void SpendManaOfColor(string color, int amount)
    {
        if (color == "any")
        {
            // Spend oldest mana regardless of color
            var spent = 0;
            for (int i = 0; i < MaxManaSlots && spent < amount; i++)
            {
                if (!ManaPool[i].IsEmpty)
                {
                    ManaPool[i] = new Mana("empty", 0);
                    spent++;
                }
            }
        }
        else
        {
            // Spend oldest of specific color
            var spent = 0;
            for (int i = 0; i < MaxManaSlots && spent < amount; i++)
            {
                if (ManaPool[i].Color == color)
                {
                    ManaPool[i] = new Mana("empty", 0);
                    spent++;
                }
            }
        }
    }

    /// <summary>
    /// Takes damage and checks for elimination.
    /// </summary>
    public void TakeDamage(int damage)
    {
        Hp -= damage;
        if (Hp <= 0)
        {
            Hp = Math.Max(0, Hp); // Don't go below 0 for display
            IsEliminated = true;
        }
    }

    /// <summary>
    /// Heals HP, cannot exceed MaxHp.
    /// </summary>
    public void Heal(int amount)
    {
        if (IsEliminated) return;
        
        Hp = Math.Min(Hp + amount, MaxHp);
    }

    /// <summary>
    /// Adds a status to the player.
    /// </summary>
    public void AddStatus(Status status)
    {
        Statuses.Add(status);
    }

    /// <summary>
    /// Decrements all status durations and removes expired ones.
    /// </summary>
    public void TickStatuses()
    {
        if (IsEliminated) return;

        foreach (var status in Statuses)
        {
            status.DecrementDuration();
        }

        Statuses.RemoveAll(s => s.ShouldRemove);
    }

    /// <summary>
    /// Checks if player has a specific status type.
    /// </summary>
    public bool HasStatus(string statusType)
    {
        return Statuses.Any(s => s.Type == statusType);
    }

    /// <summary>
    /// Gets count of a specific status type.
    /// </summary>
    public int GetStatusCount(string statusType)
    {
        return Statuses.Count(s => s.Type == statusType);
    }

    /// <summary>
    /// Checks if player is silenced (powers cannot activate).
    /// </summary>
    public bool IsSilenced => HasStatus("silence");
}

// Made with Bob
