namespace Heroes.StormReplayParser;

/// <summary>
/// /// Contains the information to parse a Heroes of the Storm replay battlelobby file.
/// </summary>
public partial class StormReplayPregame
{
    // buffers up to this size are allocated on the stack, larger ones are rented from the array pool
    private const int MaxStackAllocSize = 1024;

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
    /// <param name="licenseIdCatalog">
    /// A set of license Ids to use for parsing. This is used to determine which collection items (heroes, skins, etc...) the players in the replay have a license for.
    /// <para>This set is the same for all replays of the same build and can be obtained from <see cref="LicenseIdCatalog"/>.</para>
    /// </param>
    /// <returns>A <see cref="StormReplayPregameResult"/>.</returns>
    /// <exception cref="ArgumentException"><paramref name="path"/> cannot be <see langword="null"/> or empty.</exception>
    public static StormReplayPregameResult Parse(string path, ParsePregameOptions? parsePregameOptions = null, IReadOnlySet<long>? licenseIdCatalog = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        ParsePregameOptions options = parsePregameOptions ?? ParsePregameOptions.DefaultParsing;

        return ParseStormReplayPregame(new StormReplayPregame(path, options)
        {
            LicenseIdCatalog = licenseIdCatalog,
        });
    }

    /// <summary>
    /// Parses a <c>replay.server.battlelobby</c> stream.
    /// </summary>
    /// <param name="stream">
    /// The stream containing the <c>replay.server.battlelobby</c> data. The data is read from the stream's current position to its end.
    /// <para>The stream must support seeking, and it is disposed after parsing.</para>
    /// </param>
    /// <param name="parsePregameOptions">Sets the pregame parsing options.</param>
    /// <param name="licenseIdCatalog">
    /// A set of license Ids to use for parsing. This is used to determine which collection items (heroes, skins, etc...) the players in the replay have a license for.
    /// <para>This set is the same for all replays of the same build and can be obtained from <see cref="LicenseIdCatalog"/>.</para>
    /// </param>
    /// <returns>A <see cref="StormReplayPregameResult"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    public static StormReplayPregameResult Parse(Stream stream, ParsePregameOptions? parsePregameOptions = null, IReadOnlySet<long>? licenseIdCatalog = null)
    {
        ArgumentNullException.ThrowIfNull(stream);

        ParsePregameOptions options = parsePregameOptions ?? ParsePregameOptions.DefaultParsing;

        return ParseStormReplayPregame(new StormReplayPregame(stream, options)
        {
            LicenseIdCatalog = licenseIdCatalog,
        });
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

        // read from the current position, the stream may not be at the start
        int length = checked((int)(fileStream.Length - fileStream.Position));

        byte[]? poolBuffer = null;

        Span<byte> buffer = length <= MaxStackAllocSize
            ? stackalloc byte[MaxStackAllocSize]
            : (poolBuffer = ArrayPool<byte>.Shared.Rent(length));

        try
        {
            buffer = buffer[..length];
            fileStream.ReadExactly(buffer);

            ReplayServerBattlelobby.Parse(this, buffer, true, _parsePregameOptions.ShouldParseLicenseCatalog);
        }
        finally
        {
            if (poolBuffer is not null)
                ArrayPool<byte>.Shared.Return(poolBuffer);
        }

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
