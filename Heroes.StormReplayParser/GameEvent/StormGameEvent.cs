using Heroes.StormReplayParser.Player;
using System;

namespace Heroes.StormReplayParser.GameEvent
{
    /// <summary>
    /// Contains the information for a game event.
    /// </summary>
    public class StormGameEvent
    {
        /// <summary>
        /// Gets or sets the owner of the event.
        /// </summary>
        public StormPlayer? MessageSender { get; set; } = null;

        /// <summary>
        /// Gets or sets the time stamp of the event.
        /// </summary>
        public TimeSpan Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the type of game event.
        /// </summary>
        public StormGameEventType? GameEventType { get; set; } = null;
    }
}
