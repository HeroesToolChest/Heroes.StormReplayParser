namespace Heroes.StormReplayParser.MessageEvent;

/// <summary>
/// Contains the information for a loading progress message.
/// </summary>
public class LoadingProgressMessage : StormMessageBase
{
    /// <summary>
    /// Gets or sets the value of the loading progress.
    /// </summary>
    public int LoadingProgress { get; set; }

    /// <inheritdoc/>
    public override string Message
    {
        get
        {
            if (MessageSender == null)
                return $"({Timestamp}) ((Unknown)): [loading progress '{LoadingProgress}']";
            else if (!string.IsNullOrEmpty(MessageSender.PlayerHero?.HeroName))
                return $"({Timestamp}) {MessageSender.Name} ({MessageSender.PlayerHero.HeroName}): [loading progress '{LoadingProgress}']";
            else
                return $"({Timestamp}) {MessageSender.Name}: [loading progress '{LoadingProgress}']";
        }
    }
}
