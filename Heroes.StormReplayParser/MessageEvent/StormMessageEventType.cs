namespace Heroes.StormReplayParser.MessageEvent
{
    /// <summary>
    /// Specifies the type of message event.
    /// </summary>
    public enum StormMessageEventType
    {
        /// <summary>
        /// Indicates an unknown type.
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// Indicates a chat message.
        /// </summary>
        SChatMessage = 0,

        /// <summary>
        /// Indicates a ping message.
        /// </summary>
        SPingMessage = 1,

        /// <summary>
        /// Indicates a loading progress message.
        /// </summary>
        SLoadingProgressMessage = 2,

        /// <summary>
        /// Indicates a server ping message.
        /// </summary>
        SServerPingMessage = 3,

        /// <summary>
        /// Indicates a reconnect notify message.
        /// </summary>
        SReconnectNotifyMessage = 4,

        /// <summary>
        /// Indicates a player announce message.
        /// </summary>
        SPlayerAnnounceMessage = 5,
    }
}
