namespace Heroes.StormReplayParser.GameEvent;

/// <summary>
/// Contains the information for a game event.
/// </summary>
public struct StormGameEvent : IEquatable<StormGameEvent>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StormGameEvent"/> struct.
    /// </summary>
    /// <param name="messageSender">The player who send the message.</param>
    /// <param name="timestamp">The time the event took place.</param>
    /// <param name="gameEventType">The game event type.</param>
    /// <param name="data">Data associated with the event.</param>
    public StormGameEvent(StormPlayer? messageSender, TimeSpan timestamp, StormGameEventType? gameEventType = null, StormGameEventData? data = null)
    {
        MessageSender = messageSender;
        Timestamp = timestamp;
        GameEventType = gameEventType;
        Data = data;
    }

    /// <summary>
    /// Gets the owner of the event. If <see langword="null"/> sender is global.
    /// </summary>
    public StormPlayer? MessageSender { get; }

    /// <summary>
    /// Gets the time stamp of the event.
    /// </summary>
    public TimeSpan Timestamp { get; }

    /// <summary>
    /// Gets the type of game event.
    /// </summary>
    public StormGameEventType? GameEventType { get; }

    /// <summary>
    /// Gets the data assoicated with the event.
    /// </summary>
    public StormGameEventData? Data { get; }

    /// <summary>
    /// Compares the <paramref name="left"/> value to the <paramref name="right"/> value and determines if they are equal.
    /// </summary>
    /// <param name="left">The left hand side of the operator.</param>
    /// <param name="right">The right hand side of the operator.</param>
    /// <returns><see langword="true"/> if the <paramref name="left"/> value is equal to the <paramref name="right"/> value; otherwise <see langword="false"/>.</returns>
    public static bool operator ==(StormGameEvent? left, StormGameEvent? right)
    {
        if (left is null)
            return right is null;
        return left.Equals(right);
    }

    /// <summary>
    /// Compares the <paramref name="left"/> value to the <paramref name="right"/> value and determines if they are not equal.
    /// </summary>
    /// <param name="left">The left hand side of the operator.</param>
    /// <param name="right">The right hand side of the operator.</param>
    /// <returns><see langword="true"/> if the <paramref name="left"/> value is not equal to the <paramref name="right"/> value; otherwise <see langword="false"/>.</returns>
    public static bool operator !=(StormGameEvent? left, StormGameEvent? right)
    {
        return !(left == right);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is not StormGameEvent stormGameEvent)
            return false;
        else
            return Equals(stormGameEvent);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Timestamp, GameEventType, Data?.ToString());
    }

    /// <inheritdoc/>
    public bool Equals([AllowNull] StormGameEvent other)
    {
        return Timestamp == other.Timestamp &&
            GameEventType == other.GameEventType &&
            Data?.ToString() == other.Data?.ToString();
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        if (MessageSender == null)
            return $"[{Timestamp}] {GameEventType}: {Data}";
        else if (GameEventType == null && MessageSender == null)
            return $"[{Timestamp}]: {Data}";
        else if (GameEventType == null)
            return $"[{Timestamp}] [{MessageSender}]: {Data}";
        else
            return $"[{Timestamp}] [{MessageSender}] {GameEventType} : {Data}";
    }
}
