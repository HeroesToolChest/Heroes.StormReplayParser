namespace Heroes.StormReplayParser.MessageEvent;

/// <summary>
/// Provides the basic properties of a message.
/// </summary>
public interface IStormMessage
{
    /// <summary>
    /// Gets or sets the player that sent the message.
    /// </summary>
    public StormPlayer? MessageSender { get; set; }

    /// <summary>
    /// Gets or sets the time stamp of the message.
    /// </summary>
    public TimeSpan Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the type of message event.
    /// </summary>
    public StormMessageEventType MessageEventType { get; set; }

    /// <summary>
    /// Gets the message in the default display format.
    /// </summary>
    public string Message { get; }
}
