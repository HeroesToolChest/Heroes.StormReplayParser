namespace Heroes.StormReplayParser.MessageEvent
{
    /// <summary>
    /// Specifies a message target type.
    /// </summary>
    public enum StormMessageTarget
    {
        /// <summary>
        /// Indicates the message was for All players.
        /// </summary>
        All = 0,

        /// <summary>
        /// Indicates the message was for Ally players.
        /// </summary>
        Allies = 1,

        /// <summary>
        /// Indicates the message was for Observers.
        /// </summary>
        Observers = 4,
    }
}
