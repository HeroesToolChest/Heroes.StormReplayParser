namespace Heroes.StormReplayParser.Player;

/// <summary>
/// Contains the information for a hero talent.
/// </summary>
public class HeroTalent
{
    /// <summary>
    /// Gets or sets the slot id. This number is the zero-index of all the hero's talents, not just this talent's tier.
    /// </summary>
    public int? TalentSlotId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of this talent.
    /// </summary>
    public string? TalentNameId { get; set; }

    /// <summary>
    /// Gets or sets the time that this talent was selected.
    /// </summary>
    public TimeSpan? Timestamp { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"[{Timestamp}] {TalentNameId} - {TalentSlotId}";
    }
}
