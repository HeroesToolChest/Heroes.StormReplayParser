namespace Heroes.StormReplayParser;

/// <summary>
/// Represents the result of the storm battlelobby parsing.
/// </summary>
public class StormReplayPregameResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StormReplayPregameResult"/> class.
    /// </summary>
    /// <param name="stormReplayPregame">The parsed <see cref="StormReplayPregame"/>.</param>
    /// <param name="stormReplayPregameParseStatus">The <see cref="StormReplayPregameParseStatus"/>.</param>
    /// <param name="exception">The exception, if any.</param>
    internal StormReplayPregameResult(StormReplayPregame stormReplayPregame, StormReplayPregameParseStatus stormReplayPregameParseStatus, StormParseException? exception = null)
    {
        ReplayBattleLobby = stormReplayPregame;
        Status = stormReplayPregameParseStatus;
        Exception = exception;
    }

    /// <summary>
    /// Gets the status of the parsed battlelobby file.
    /// </summary>
    public StormReplayPregameParseStatus Status { get; }

    /// <summary>
    /// Gets the exception, if any, from the parsed battlelobby file.
    /// </summary>
    public StormParseException? Exception { get; } = null;

    /// <summary>
    /// Gets the parsed <see cref="StormReplayPregame"/>.
    /// </summary>
    public StormReplayPregame ReplayBattleLobby { get; }
}
