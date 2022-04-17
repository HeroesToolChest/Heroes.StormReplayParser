namespace Heroes.StormReplayParser.Player;

/// <summary>
/// Contains the properties for a hero master tiers.
/// </summary>
public class HeroMasteryTier
{
    /// <summary>
    /// Gets or sets the hero attribute id.
    /// </summary>
    public string HeroAttributeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tier level.
    /// </summary>
    public int TierLevel { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{HeroAttributeId}: {TierLevel}";
    }
}
