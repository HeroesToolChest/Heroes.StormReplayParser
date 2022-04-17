namespace Heroes.StormReplayParser.MessageEvent;

/// <summary>
/// Contains the information for a reconnect notify message.
/// </summary>
public class ReconnectNotifyMessage : StormMessageBase
{
    /// <summary>
    /// Gets or sets the value of the status.
    /// </summary>
    public int Status { get; set; }

    /// <inheritdoc/>
    public override string Message
    {
        get
        {
            if (MessageSender == null)
                return $"({Timestamp}) ((Unknown)): [reconnect status '{Status}']";
            else if (!string.IsNullOrEmpty(MessageSender.PlayerHero?.HeroName))
                return $"({Timestamp}) {MessageSender.Name} ({MessageSender.PlayerHero.HeroName}): [reconnect status '{Status}']";
            else
                return $"({Timestamp}) {MessageSender.Name}: [reconnect status '{Status}']";
        }
    }
}
