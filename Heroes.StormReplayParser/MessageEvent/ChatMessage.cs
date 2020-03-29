namespace Heroes.StormReplayParser.MessageEvent
{
    /// <summary>
    /// Contains the information for a chat message.
    /// </summary>
    public class ChatMessage
    {
        /// <summary>
        /// Gets or sets the target of the message.
        /// </summary>
        public StormMessageTarget MessageTarget { get; set; }

        /// <summary>
        /// Gets or sets the message sent.
        /// </summary>
        public string? Message { get; set; } = null;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[{MessageTarget}] {Message}";
        }
    }
}
