namespace Heroes.StormReplayParser.Player;

/// <summary>
/// Contains the information for a hero talent.
/// </summary>
public class HeroTalent
{
    /// <summary>
    /// Gets or sets the slot id. This number is the zero-index of all the hero's talents, not just this talent's tier.
    /// <para>
    /// This requires <see cref="ParseOptions.ShouldParseGameEvents"/>. If it is <see langword="false"/>, this will be <see langword="null"/>.
    /// </para>
    /// </summary>
    public int? TalentSlotId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of this talent.
    /// <para>
    /// This is the <c>id</c> value of the <c>CTalent</c> element in the game data.
    /// </para>
    /// <para>
    /// This requires <see cref="ParseOptions.ShouldParseTrackerEvents"/>. If it is <see langword="false"/>, this will be <see langword="null"/>.
    /// </para>
    /// </summary>
    public string? TalentNameId { get; set; }

    /// <summary>
    /// Gets or sets the time that this talent was selected.
    /// <para>
    /// This requires <see cref="ParseOptions.ShouldParseGameEvents"/>. If it is <see langword="false"/>, this will be <see langword="null"/>.
    /// </para>
    /// </summary>
    public TimeSpan? Timestamp { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"[{Timestamp}] {TalentNameId} - {TalentSlotId}";
    }
}
