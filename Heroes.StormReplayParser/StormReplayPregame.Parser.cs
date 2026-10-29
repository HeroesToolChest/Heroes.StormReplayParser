namespace Heroes.StormReplayParser;

/// <summary>
/// Contains the information to parse a Heroes of the Storm replay battlelobby file.
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
    /// <para>
    /// Errors from opening or parsing the file are not thrown. Instead, the result's <see cref="StormReplayPregameResult.Status"/> is <see cref="StormReplayPregameParseStatus.Exception"/>
    /// and <see cref="StormReplayPregameResult.Exception"/> has the error. This includes a file that is not a battlelobby file.
    /// </para>
    /// </summary>
    /// <param name="path">The path to the <c>replay.server.battlelobby</c> file. The file is opened for reading and closed after parsing.</param>
    /// <param name="parsePregameOptions">The pregame parsing options. If <see langword="null"/>, <see cref="ParsePregameOptions.DefaultParsing"/> is used.</param>
    /// <param name="licenseIdCatalog">
    /// A license id catalog, which is the set of all the license ids of a build, such as the <see cref="LicenseIdCatalog"/> of another battlelobby file of the same build.
    /// <para>
    /// If one is passed, the players' license ids are always parsed, even if <see cref="ParsePregameOptions.ShouldParseLicenseCatalog"/> is <see langword="false"/>.
    /// If it matches the battlelobby file's catalog, it is used as this <see cref="LicenseIdCatalog"/>, so battlelobby files of the same build can share one set.
    /// If it does not match, such as a catalog from a different build, a new set is created from the battlelobby file.
    /// </para>
    /// </param>
    /// <returns>A <see cref="StormReplayPregameResult"/> with the parsed battlelobby file and its parse status.</returns>
    /// <exception cref="ArgumentException"><paramref name="path"/> is <see langword="null"/>, empty, or only whitespace.</exception>
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
    /// <para>
    /// Errors from reading or parsing the data are not thrown. Instead, the result's <see cref="StormReplayPregameResult.Status"/> is <see cref="StormReplayPregameParseStatus.Exception"/>
    /// and <see cref="StormReplayPregameResult.Exception"/> has the error. This includes data that is not a battlelobby file.
    /// </para>
    /// </summary>
    /// <param name="stream">
    /// The stream containing the <c>replay.server.battlelobby</c> data. The data is read from the stream's current position to its end.
    /// <para>The stream must support seeking, and it is disposed after parsing.</para>
    /// </param>
    /// <param name="parsePregameOptions">The pregame parsing options. If <see langword="null"/>, <see cref="ParsePregameOptions.DefaultParsing"/> is used.</param>
    /// <param name="licenseIdCatalog">
    /// A license id catalog, which is the set of all the license ids of a build, such as the <see cref="LicenseIdCatalog"/> of another battlelobby file of the same build.
    /// <para>
    /// If one is passed, the players' license ids are always parsed, even if <see cref="ParsePregameOptions.ShouldParseLicenseCatalog"/> is <see langword="false"/>.
    /// If it matches the battlelobby file's catalog, it is used as this <see cref="LicenseIdCatalog"/>, so battlelobby files of the same build can share one set.
    /// If it does not match, such as a catalog from a different build, a new set is created from the battlelobby file.
    /// </para>
    /// </param>
    /// <returns>A <see cref="StormReplayPregameResult"/> with the parsed battlelobby file and its parse status.</returns>
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
