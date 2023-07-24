namespace Heroes.StormReplayParser.Pregame.Player;

/// <summary>
/// Contains the information for a player's hero during the pregame.
/// </summary>
public class PregamePlayerHero
{
    /// <summary>
    /// Gets or sets the hero attribute id. Not recommended to use an identifier for certain brawl maps as
    /// this will be set as the pre-selected hero.
    /// </summary>
    public string HeroAttributeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the hero's level. This will be a 1 if the hero is auto selected (<see cref="StormPlayer.IsAutoSelect"/>).
    /// </summary>
    public int HeroLevel { get; set; } = 0;

    /// <inheritdoc/>
    public override string ToString() => HeroAttributeId;
}
