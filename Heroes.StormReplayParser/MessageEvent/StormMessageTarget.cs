namespace Heroes.StormReplayParser.MessageEvent;

/// <summary>
/// Indicates the target message channel.
/// </summary>
public enum StormMessageTarget
{
    /// <summary>
    /// Indicates the message target is unknown.
    /// </summary>
    Unknown = -1,

    /// <summary>
    /// Indicates the message was for the <c>All</c> message channel.
    /// </summary>
    All = 0,

    /// <summary>
    /// Indicates the message was for the <c>Allies</c> message channel.
    /// </summary>
    Allies = 1,

    /// <summary>
    /// Indicates the message was for the <c>Observers</c> message channel.
    /// </summary>
    Observers = 4,

    /// <summary>
    /// Indicates the message was for the <c>Enemies</c> message channel.
    /// </summary>
    Enemies = 10,
}
