using Heroes.StormReplayParser.Player;
using System;

namespace Heroes.StormReplayParser.MessageEvent
{
    /// <summary>
    /// Contains the information for a message.
    /// </summary>
    public class StormMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StormMessage"/> class.
        /// </summary>
        /// <param name="chatMessage">The chat message.</param>
        public StormMessage(ChatMessage chatMessage)
        {
            ChatMessage = chatMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StormMessage"/> class.
        /// </summary>
        /// <param name="pingMessage">The ping message.</param>
        public StormMessage(PingMessage pingMessage)
        {
            PingMessage = pingMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StormMessage"/> class.
        /// </summary>
        /// <param name="loadingProgressMessage">The loading progress message.</param>
        public StormMessage(LoadingProgressMessage loadingProgressMessage)
        {
            LoadingProgressMessage = loadingProgressMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StormMessage"/> class.
        /// </summary>
        /// <param name="reconnectNotifyMessage">The reconnect notify message.</param>
        public StormMessage(ReconnectNotifyMessage reconnectNotifyMessage)
        {
            ReconnectNotifyMessage = reconnectNotifyMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StormMessage"/> class.
        /// </summary>
        /// <param name="playerAnnounceMessage">The player announce message.</param>
        public StormMessage(PlayerAnnounceMessage playerAnnounceMessage)
        {
            PlayerAnnounceMessage = playerAnnounceMessage;
        }

        /// <summary>
        /// Gets or sets the message sender player.
        /// </summary>
        public StormPlayer? MessageSender { get; set; } = null;

        /// <summary>
        /// Gets or sets the time stamp of the message.
        /// </summary>
        public TimeSpan Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the type of message event.
        /// </summary>
        public StormMessageEventType? MessageEventType { get; set; } = null;

        /// <summary>
        /// Gets the chat message.
        /// </summary>
        public ChatMessage? ChatMessage { get; } = null;

        /// <summary>
        /// Gets the ping message.
        /// </summary>
        public PingMessage? PingMessage { get; } = null;

        /// <summary>
        /// Gets the loading progress message.
        /// </summary>
        public LoadingProgressMessage? LoadingProgressMessage { get; } = null;

        /// <summary>
        /// Gets the reconnect notify message.
        /// </summary>
        public ReconnectNotifyMessage? ReconnectNotifyMessage { get; } = null;

        /// <summary>
        /// Gets the player announce message.
        /// </summary>
        public PlayerAnnounceMessage? PlayerAnnounceMessage { get; } = null;

        /// <inheritdoc/>
        public override string? ToString()
        {
            if (MessageEventType == null)
                return null;

            return MessageEventType switch
            {
                StormMessageEventType.SChatMessage => GetChatMessage(),
                StormMessageEventType.SPingMessage => GetPingMessage(),
                StormMessageEventType.SLoadingProgressMessage => GetLoadingProgressMessage(),
                StormMessageEventType.SReconnectNotifyMessage => GetReconnectNotifyMessage(),
                StormMessageEventType.SPlayerAnnounceMessage => GetPlayerAnnounceMessage(),
                _ => base.ToString(),
            };
        }

        private string GetChatMessage()
        {
            if (MessageSender == null)
                return $"({Timestamp}) [{ChatMessage!.MessageTarget}] ((Unknown)): {ChatMessage.Message}";
            else if (!string.IsNullOrEmpty(MessageSender.PlayerHero.HeroName))
                return $"({Timestamp}) [{ChatMessage!.MessageTarget}] {MessageSender.Name} ({MessageSender.PlayerHero.HeroName}): {ChatMessage.Message}";
            else
                return $"({Timestamp}) [{ChatMessage!.MessageTarget}] {MessageSender.Name}: {ChatMessage.Message}";
        }

        private string GetPingMessage()
        {
            if (MessageSender == null)
                return $"({Timestamp}) [{PingMessage!.MessageTarget}] ((Unknown)) used a ping";
            else if (!string.IsNullOrEmpty(MessageSender.PlayerHero.HeroName))
                return $"({Timestamp}) [{PingMessage!.MessageTarget}] {MessageSender.Name} ({MessageSender.PlayerHero.HeroName}) used a ping";
            else
                return $"({Timestamp}) [{PingMessage!.MessageTarget}] {MessageSender.Name} used a ping";
        }

        private string GetLoadingProgressMessage()
        {
            if (MessageSender == null)
                return $"({Timestamp}) ((Unknown)) loading progress: {LoadingProgressMessage!.LoadingProgress}";
            else if (!string.IsNullOrEmpty(MessageSender.PlayerHero.HeroName))
                return $"({Timestamp}) {MessageSender.Name} ({MessageSender.PlayerHero.HeroName}) loading progress: {LoadingProgressMessage!.LoadingProgress}";
            else
                return $"({Timestamp}) {MessageSender.Name} loading progress: {LoadingProgressMessage!.LoadingProgress}";
        }

        private string GetReconnectNotifyMessage()
        {
            if (MessageSender == null)
                return $"({Timestamp}) ((Unknown)) reconnect status: {ReconnectNotifyMessage!.Status}";
            else if (!string.IsNullOrEmpty(MessageSender.PlayerHero.HeroName))
                return $"({Timestamp}) {MessageSender.Name} ({MessageSender.PlayerHero.HeroName}) reconnect status: {ReconnectNotifyMessage!.Status}";
            else
                return $"({Timestamp}) {MessageSender.Name} reconnect status: {ReconnectNotifyMessage!.Status}";
        }

        private string GetPlayerAnnounceMessage()
        {
            if (MessageSender == null)
                return $"({Timestamp}) ((Unknown)) announced {PlayerAnnounceMessage!.AnnouncementType}";
            else if (!string.IsNullOrEmpty(MessageSender.PlayerHero.HeroName))
                return $"({Timestamp}) {MessageSender.Name} ({MessageSender.PlayerHero.HeroName}) announced {PlayerAnnounceMessage!.AnnouncementType}";
            else
                return $"({Timestamp}) {MessageSender.Name} announced {PlayerAnnounceMessage!.AnnouncementType}";
        }
    }
}
