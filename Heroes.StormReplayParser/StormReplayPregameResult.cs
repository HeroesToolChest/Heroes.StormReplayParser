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
    /// <param name="fileName">The file name of the heroes of the storm battlelobby file.</param>
    /// <param name="exception">The exception, if any.</param>
    /// <exception cref="ArgumentException"><paramref name="fileName"/> is <see langword="null"/> or emtpy.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="fileName"/> or <paramref name="stormReplayPregame"/> is <see langword="null"/>.</exception>
    internal StormReplayPregameResult(StormReplayPregame stormReplayPregame, StormReplayPregameParseStatus stormReplayPregameParseStatus, string fileName, StormParseException? exception = null)
    {
        ReplayBattleLobby = stormReplayPregame ?? throw new ArgumentNullException(nameof(stormReplayPregame));
        Status = stormReplayPregameParseStatus;
        FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
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

    /// <summary>
    /// Gets the file name (includes the path).
    /// </summary>
    public string FileName { get; }
}
