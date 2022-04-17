namespace Heroes.StormReplayParser;

/// <summary>
/// Contains the methods for a storm parse exception.
/// </summary>
public class StormParseException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StormParseException"/> class.
    /// </summary>
    public StormParseException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StormParseException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public StormParseException(string? message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StormParseException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The exception.</param>
    public StormParseException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
