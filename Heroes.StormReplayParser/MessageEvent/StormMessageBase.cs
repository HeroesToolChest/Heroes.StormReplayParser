using Heroes.StormReplayParser.Player;
using System;

namespace Heroes.StormReplayParser.MessageEvent
{
    /// <summary>
    /// An abstract base class for messages.
    /// </summary>
    public abstract class StormMessageBase : IStormMessage
    {
        /// <inheritdoc/>
        public StormPlayer? MessageSender { get; set; }

        /// <inheritdoc/>
        public TimeSpan Timestamp { get; set; }

        /// <inheritdoc/>
        public StormMessageEventType MessageEventType { get; set; } = StormMessageEventType.Unknown;

        /// <inheritdoc/>
        public abstract string Message { get; }

        /// <inheritdoc/>
        public override string ToString() => Message;
    }
}
