namespace Heroes.StormReplayParser.Player
{
    /// <summary>
    /// Contains the information for a player's hero.
    /// </summary>
    public class PlayerHero
    {
        /// <summary>
        /// Gets or sets the hero id.
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
        /// Gets or sets the hero attribute id.
        /// </summary>
        public string HeroAttributeId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the hero's level.
        /// </summary>
        public int HeroLevel { get; set; } = 0;

        /// <inheritdoc/>
        public override string? ToString()
        {
            return $"{HeroId}-{HeroName}";
        }
    }
}
