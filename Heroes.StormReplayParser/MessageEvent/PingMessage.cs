namespace Heroes.StormReplayParser.MessageEvent;

/// <summary>
/// Contains the information for a ping message.
/// </summary>
/// <remarks>
/// Ping messages include normal pings (no target), targeted pings (such as Player 1 wants to help Player 2), retreat,
/// and the more ping options (on my way, defend, danger, assist)
/// does not include captured camps, hearthing, no way to tell which one is which.
/// </remarks>
public class PingMessage : StormMessageBase
{
    /// <summary>
    /// Gets or sets the target of the message.
    /// </summary>
    public StormMessageTarget MessageTarget { get; set; }

    /// <summary>
    /// Gets or sets the coordinates of where the ping message was targeted at.
    /// </summary>
    public Point Point { get; set; }

    /// <inheritdoc/>
    public override string Message
    {
        get
        {
            if (MessageSender == null)
                return $"({Timestamp}) [{MessageTarget}] ((Unknown)): ['PING']";
            else if (!string.IsNullOrEmpty(MessageSender.PlayerHero?.HeroName))
                return $"({Timestamp}) [{MessageTarget}] {MessageSender.Name} ({MessageSender.PlayerHero.HeroName}): ['PING']";
            else
                return $"({Timestamp}) [{MessageTarget}] {MessageSender.Name}: ['PING']";
        }
    }
}
