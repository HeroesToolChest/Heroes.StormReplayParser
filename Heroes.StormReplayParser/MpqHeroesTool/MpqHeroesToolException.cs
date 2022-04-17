namespace Heroes.StormReplayParser.MpqHeroesTool;

/// <summary>
/// Represents mpq parser exceptions.
/// </summary>
public class MpqHeroesToolException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MpqHeroesToolException"/> class.
    /// </summary>
    public MpqHeroesToolException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MpqHeroesToolException"/> class.
    /// </summary>
    /// <param name="message">The custom error message of the exception.</param>
    public MpqHeroesToolException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MpqHeroesToolException"/> class.
    /// </summary>
    /// <param name="message">The custom error message of the exception.</param>
    /// <param name="innerException">The <see cref="Exception"/> that occured.</param>
    public MpqHeroesToolException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
