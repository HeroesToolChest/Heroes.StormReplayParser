namespace Heroes.StormReplayParser.Player
{
    /// <summary>
    /// Specifies the type of player.
    /// </summary>
    public enum PlayerType
    {
        /// <summary>
        /// Indicates a human player.
        /// </summary>
        Human,

        /// <summary>
        /// Indicates an AI player.
        /// </summary>
        Computer,

        /// <summary>
        /// Indicates a non-player who is spectating.
        /// </summary>
        Observer,
    }
}
