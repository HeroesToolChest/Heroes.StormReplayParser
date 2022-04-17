namespace Heroes.StormReplayParser.MessageEvent;

/// <summary>
/// Contains the information for a chat message.
/// </summary>
public class ChatMessage : StormMessageBase
{
    /// <summary>
    /// Gets or sets the target of the message.
    /// </summary>
    public StormMessageTarget MessageTarget { get; set; }

    /// <summary>
    /// Gets or sets the text sent by the player.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <inheritdoc/>
    public override string Message
    {
        get
        {
            if (MessageSender == null)
                return $"({Timestamp}) [{MessageTarget}] ((Unknown)): {Text}";
            else if (!string.IsNullOrEmpty(MessageSender.PlayerHero?.HeroName))
                return $"({Timestamp}) [{MessageTarget}] {MessageSender.Name} ({MessageSender.PlayerHero.HeroName}): {Text}";
            else
                return $"({Timestamp}) [{MessageTarget}] {MessageSender.Name}: {Text}";
        }
    }
}
