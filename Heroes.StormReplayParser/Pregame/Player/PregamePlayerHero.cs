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
    /// </summary>
    public int HeroLevel { get; set; } = 0;

    /// <summary>
    /// Gets or sets the hero's mastery tier level.
    /// </summary>
    public int? HeroMasteryTier { get; set; }

    /// <inheritdoc/>
    public override string ToString() => HeroAttributeId;
}
