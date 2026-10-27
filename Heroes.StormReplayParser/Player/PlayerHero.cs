namespace Heroes.StormReplayParser.Player;

/// <summary>
/// Contains the information for a player's hero.
/// </summary>
public class PlayerHero
{
    /// <summary>
    /// Gets or sets the hero id. Not recommended to use an identifier as certain Brawl maps and ARAM will have
    /// this set to be the pre-selected hero. For AI games, this will be empty.
    /// <para>
    /// This is the <c>id</c> value of the <c>CHero</c> element in the game data.
    /// </para>
    /// </summary>
    public string HeroId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the hero unit id. This is set from tracker events.
    /// <para>
    /// This is the <c>id</c> value of the <c>CUnit</c> element in the game data.
    /// </para>
    /// </summary>
    public string HeroUnitId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the hero name (this name is localized).
    /// </summary>
    public string HeroName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the hero attribute id. Not recommended to use an identifier as certain Brawl maps and ARAM will have
    /// this set to be the pre-selected hero. For AI games, this will be empty.
    /// <para>
    /// This is the <c>AttributeId</c> value of the <c>CHero</c> element in the game data.
    /// </para>
    /// </summary>
    public string HeroAttributeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the hero's level. This will be a 1 if the hero is auto selected (<see cref="StormPlayer.IsAutoSelect"/>).
    /// <para>
    /// This is capped at 20, use <see cref="HeroMasteryTier.TierLevel"/> to calculate a hero's minimal level.
    /// </para>
    /// </summary>
    public int HeroLevel { get; set; } = 0;

    /// <summary>
    /// Gets or sets the tier of the hero mastery ring that the player has equipped on the hero. This is a tier from 1 to 3.
    /// <para>
    /// This will be <see langword="null"/> if no mastery ring is equipped.
    /// </para>
    /// </summary>
    public int? HeroMasteryRingTier { get; set; }

    /// <inheritdoc/>
    public override string? ToString()
    {
        if (!string.IsNullOrEmpty(HeroUnitId))
            return $"UnitId: {HeroUnitId}-{HeroName}";
        else if (!string.IsNullOrEmpty(HeroId))
            return $"HeroId: {HeroId}-{HeroName}";
        else
            return HeroName;
    }
}
