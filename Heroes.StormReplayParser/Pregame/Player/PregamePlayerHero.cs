namespace Heroes.StormReplayParser.Pregame.Player;

/// <summary>
/// Contains the information for a player's hero during the pregame.
/// </summary>
public class PregamePlayerHero
{
    /// <summary>
    /// Gets or sets the hero attribute id. For certain Brawl maps and ARAM this is the pre-selected hero.
    /// </summary>
    public string HeroAttributeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the hero's level. This will be a 1 if the hero is auto selected (<see cref="StormPlayer.IsAutoSelect"/>).
    /// <para>
    /// This is capped at 20, use <see cref="HeroMasteryTier"/> to calculate a hero's minimal level.
    /// </para>
    /// </summary>
    public int HeroLevel { get; set; } = 0;

    /// <summary>
    /// Gets or sets the tier level for the hero taunt mastery. The tier level is granted based on a hero's level.
    /// <para><c>Tier 1 - Level 15</c>.</para>
    /// <para><c>Tier 2 - Level 25</c>.</para>
    /// <para><c>Tier 3 - Level 50</c>.</para>
    /// <para><c>Tier 4 - Level 75</c>.</para>
    /// <para><c>Tier 5 - Level 100</c>.</para>
    /// </summary>
    public int? HeroMasteryTier { get; set; }

    /// <summary>
    /// Gets or sets the tier of the hero mastery ring that the player has equipped on the hero. This is a tier from 1 to 3.
    /// <para>
    /// This will be <see langword="null"/> if no mastery ring is equipped.
    /// </para>
    /// </summary>
    public int? HeroMasteryRingTier { get; set; }

    /// <inheritdoc/>
    public override string ToString() => HeroAttributeId;
}
