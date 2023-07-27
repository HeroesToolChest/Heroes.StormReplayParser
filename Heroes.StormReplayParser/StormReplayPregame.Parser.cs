namespace Heroes.StormReplayParser;

/// <summary>
/// /// Contains the information to parse a Heroes of the Storm replay battlelobby file.
/// </summary>
public partial class StormReplayPregame
{
    private static StormReplayPregameParseStatus _stormReplayPregameParseResult = StormReplayPregameParseStatus.Unknown;
    private static StormParseException? _failedReplayException = null;

    private readonly string _fileName;
    private readonly ParsePregameOptions _parsePregameOptions;

    internal StormReplayPregame()
    {
        _fileName = string.Empty;
        _parsePregameOptions = ParsePregameOptions.DefaultParsing;
    }

    private StormReplayPregame(string fileName, ParsePregameOptions parsePregameOptions)
    {
        _fileName = fileName;
        _parsePregameOptions = parsePregameOptions;
    }

    /// <summary>
    /// Parses a replay.server.battlelobby file.
    /// </summary>
    /// <param name="fileName">The file name which may contain the path.</param>
    /// <param name="parsePregameOptions">Sets the parsing options. If <see cref="ParseOptions.AllowPTR"/> is <see langword="false"/> the result status will be <see cref="StormReplayParseStatus.PTRRegion"/> if the replay is successfully parsed.</param>
    /// <returns>A <see cref="StormReplayPregameResult"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is null.</exception>
    public static StormReplayPregameResult Parse(string fileName, ParsePregameOptions? parsePregameOptions = null)
    {
        if (fileName is null)
            throw new ArgumentNullException(nameof(fileName));

        parsePregameOptions ??= ParsePregameOptions.DefaultParsing;

        StormReplayPregame stormReplayPregame = ParseStormReplayPregame(fileName, parsePregameOptions);

        return new StormReplayPregameResult(stormReplayPregame, _stormReplayPregameParseResult, fileName, _failedReplayException);
    }

    private static StormReplayPregame ParseStormReplayPregame(string fileName, ParsePregameOptions parsePregameOptions)
    {
        StormReplayPregame stormReplayPregame = new(fileName, parsePregameOptions);

        try
        {
            stormReplayPregame.Parse(stormReplayPregame);
        }
        catch (Exception exception)
        {
            _failedReplayException = new StormParseException("An exception has occured during the parsing of the battlelobby.", exception);
            _stormReplayPregameParseResult = StormReplayPregameParseStatus.Exception;
        }

        return stormReplayPregame;
    }

    private void Parse(StormReplayPregame stormReplayPregame)
    {
        using FileStream fileStream = new(_fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

        Span<byte> buffer = stackalloc byte[(int)fileStream.Length];
        fileStream.Read(buffer);

        ReplayServerBattlelobby.Parse(stormReplayPregame, buffer, true);

        ValidateResult(stormReplayPregame);
    }

    private void ValidateResult(StormReplayPregame stormReplayPregame)
    {
        if (!_parsePregameOptions.AllowPTR && stormReplayPregame.StormPlayers.Any(x => x.ToonHandle?.Region >= 90))
            _stormReplayPregameParseResult = StormReplayPregameParseStatus.PTRRegion;
        else
            _stormReplayPregameParseResult = StormReplayPregameParseStatus.Success;
    }
}
