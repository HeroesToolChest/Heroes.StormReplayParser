namespace Heroes.StormReplayParser.Player
{
    /// <summary>
    /// Contains the information for a player's hero.
    /// </summary>
    public class PlayerHero
    {
        /// <summary>
        /// Gets or sets the hero id. Not recommended to use an identifier in certain brawl maps as
        /// this will be set as the pre-selected hero.
        /// </summary>
        public string HeroId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the hero unit id. This is set from tracker events.
        /// </summary>
        public string HeroUnitId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the hero name (this is a localized name).
        /// </summary>
        public string HeroName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the hero attribute id. Not recommended to use an identifier in certain brawl maps as
        /// this will be set as the pre-selected hero.
        /// </summary>
        public string HeroAttributeId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the hero's level. This will be a 1 if the hero is auto selected (<see cref="StormPlayer.IsAutoSelect"/>).
        /// </summary>
        public int HeroLevel { get; set; } = 0;

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
}
