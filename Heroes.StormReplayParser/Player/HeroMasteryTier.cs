namespace Heroes.StormReplayParser.Player;

/// <summary>
/// Contains the properties for a hero taunt mastery tiers.
/// </summary>
public class HeroMasteryTier
{
    /// <summary>
    /// Gets or sets the hero attribute id.
    /// </summary>
    public string HeroAttributeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tier level for the hero taunt mastery. The tier level is granted based on a hero's level.
    /// <para><c>Tier 1 - Level 15</c>.</para>
    /// <para><c>Tier 2 - Level 25</c>.</para>
    /// <para><c>Tier 3 - Level 50</c>.</para>
    /// <para><c>Tier 4 - Level 75</c>.</para>
    /// <para><c>Tier 5 - Level 100</c>.</para>
    /// </summary>
    public int TierLevel { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{HeroAttributeId}: {TierLevel}";
    }
}
