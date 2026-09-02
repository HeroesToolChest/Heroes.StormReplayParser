namespace Heroes.StormReplayParser;

/// <summary>
/// /// Contains the information to parse a Heroes of the Storm replay battlelobby file.
/// </summary>
public partial class StormReplayPregame
{
    private readonly ParsePregameOptions _parsePregameOptions;
    private readonly Stream? _battlelobbyStream;

    private StormReplayPregameParseStatus _parseStatus = StormReplayPregameParseStatus.Unknown;
    private StormParseException? _failedReplayException;

    internal StormReplayPregame()
    {
        _parsePregameOptions = ParsePregameOptions.DefaultParsing;
    }

    private StormReplayPregame(string path, ParsePregameOptions parsePregameOptions)
    {
        try
        {
            _battlelobbyStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
        catch (Exception exception)
        {
            SetFailed(exception);
        }

        _parsePregameOptions = parsePregameOptions;
    }

    private StormReplayPregame(Stream stream, ParsePregameOptions parsePregameOptions)
    {
        _battlelobbyStream = stream;
        _parsePregameOptions = parsePregameOptions;
    }

    /// <summary>
    /// Parses a <c>replay.server.battlelobby</c> file.
    /// </summary>
    /// <param name="path">The path to the <c>replay.server.battlelobby</c> file.</param>
    /// <param name="parsePregameOptions">Sets the pregame parsing options.</param>
    /// <returns>A <see cref="StormReplayPregameResult"/>.</returns>
    /// <exception cref="ArgumentException"><paramref name="path"/> cannot be <see langword="null"/> or empty.</exception>
    public static StormReplayPregameResult Parse(string path, ParsePregameOptions? parsePregameOptions = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        return ParseStormReplayPregame(new StormReplayPregame(path, parsePregameOptions ?? ParsePregameOptions.DefaultParsing));
    }

    /// <summary>
    /// Parses a <c>replay.server.battlelobby</c> stream.
    /// </summary>
    /// <param name="stream">The stream containing the <c>replay.server.battlelobby</c> data.</param>
    /// <param name="parsePregameOptions">Sets the pregame parsing options.</param>
    /// <returns>A <see cref="StormReplayPregameResult"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    public static StormReplayPregameResult Parse(Stream stream, ParsePregameOptions? parsePregameOptions = null)
    {
        ArgumentNullException.ThrowIfNull(stream);

        return ParseStormReplayPregame(new StormReplayPregame(stream, parsePregameOptions ?? ParsePregameOptions.DefaultParsing));
    }

    private static StormReplayPregameResult ParseStormReplayPregame(StormReplayPregame stormReplayPregame)
    {
        try
        {
            stormReplayPregame.Parse();
        }
        catch (Exception exception)
        {
            stormReplayPregame.SetFailed(exception);
        }

        return new StormReplayPregameResult(stormReplayPregame, stormReplayPregame._parseStatus, stormReplayPregame._failedReplayException);
    }

    private void SetFailed(Exception exception)
    {
        _failedReplayException = new StormParseException("An exception has occurred during the parsing of the battlelobby.", exception);
        _parseStatus = StormReplayPregameParseStatus.Exception;
    }

    private void Parse()
    {
        if (_battlelobbyStream is null)
            return;

        using Stream fileStream = _battlelobbyStream;

        Span<byte> buffer = stackalloc byte[(int)fileStream.Length];
        fileStream.ReadExactly(buffer);

        ReplayServerBattlelobby.Parse(this, buffer, true);

        ValidateResult();
    }

    private void ValidateResult()
    {
        if (!_parsePregameOptions.AllowPTR && StormPlayers.Any(x => x.ToonHandle?.Region >= 90))
            _parseStatus = StormReplayPregameParseStatus.PTRRegion;
        else
            _parseStatus = StormReplayPregameParseStatus.Success;
    }
}
