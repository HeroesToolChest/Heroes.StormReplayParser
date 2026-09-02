namespace Heroes.StormReplayParser;

/// <summary>
/// Contains the information to parse a Heroes of the Storm replay.
/// </summary>
public partial class StormReplay
{
    private readonly ParseOptions _parseOptions;
    private readonly MpqHeroesArchive? _stormMpqArchive;

    private StormReplayParseStatus _parseStatus = StormReplayParseStatus.Incomplete;
    private StormParseException? _failedReplayException;

    private StormReplay(string path, ParseOptions parseOptions)
    {
        _parseOptions = parseOptions;

        try
        {
            _stormMpqArchive = MpqHeroesFile.Open(path);
        }
        catch (Exception exception)
        {
            SetFailed(exception);
        }
    }

    private StormReplay(Stream stream, ParseOptions parseOptions)
    {
        _parseOptions = parseOptions;

        try
        {
            _stormMpqArchive = MpqHeroesFile.Open(stream);
        }
        catch (Exception exception)
        {
            SetFailed(exception);
        }
    }

    private delegate void MpqFileParser(StormReplay replay, ReadOnlySpan<byte> source);

    /// <summary>
    /// Parses a <c>.StormReplay</c> file.
    /// </summary>
    /// <param name="path">The path to the <c>.StormReplay</c> file.</param>
    /// <param name="parseOptions">Sets the parsing options. If <see cref="ParseOptions.AllowPTR"/> is <see langword="false"/> the result status will be <see cref="StormReplayParseStatus.PTRRegion"/> if the replay is successfully parsed.</param>
    /// <returns>A <see cref="StormReplayResult"/>.</returns>
    /// <exception cref="ArgumentException"><paramref name="path"/> cannot be <see langword="null"/> or empty.</exception>
    public static StormReplayResult Parse(string path, ParseOptions? parseOptions = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        return ParseStormReplay(new StormReplay(path, parseOptions ?? ParseOptions.DefaultParsing));
    }

    /// <summary>
    /// Parses a <c>.StormReplay</c> stream.
    /// </summary>
    /// <param name="stream">The stream containing the <c>.StormReplay</c> data.</param>
    /// <param name="parseOptions">Sets the parsing options. If <see cref="ParseOptions.AllowPTR"/> is <see langword="false"/> the result status will be <see cref="StormReplayParseStatus.PTRRegion"/> if the replay is successfully parsed.</param>
    /// <returns>A <see cref="StormReplayResult"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    public static StormReplayResult Parse(Stream stream, ParseOptions? parseOptions = null)
    {
        ArgumentNullException.ThrowIfNull(stream);

        return ParseStormReplay(new StormReplay(stream, parseOptions ?? ParseOptions.DefaultParsing));
    }

    private static StormReplayResult ParseStormReplay(StormReplay stormReplay)
    {
        try
        {
            stormReplay.Parse();
        }
        catch (Exception exception)
        {
            stormReplay.SetFailed(exception);
        }

        return new StormReplayResult(stormReplay, stormReplay._parseStatus, stormReplay._failedReplayException);
    }

    private void SetFailed(Exception exception)
    {
        _failedReplayException = new StormParseException("An exception has occurred during the parsing of the replay.", exception);
        _parseStatus = StormReplayParseStatus.Exception;
    }

    private void FinalPlayerData()
    {
        TimeSpan latestCameraUpdateEvent = TimeSpan.MinValue;
        bool foundPlayer = false;

        foreach (StormPlayer? player in ClientListByUserID)
        {
            if (player is null)
                continue;

            foundPlayer = true;

            if (player.LastCameraUpdateEvent > latestCameraUpdateEvent)
                latestCameraUpdateEvent = player.LastCameraUpdateEvent;
        }

        if (!foundPlayer)
            throw new InvalidOperationException("Sequence contains no elements");

        // remove the occurrence where the players leaves at the end of the match
        foreach (StormPlayer? player in ClientListByUserID)
        {
            if (player is null)
                continue;

            List<PlayerDisconnect> disconnects = player.PlayerDisconnectsInternal;

            if (disconnects.Count > 0 && disconnects[^1].From > latestCameraUpdateEvent)
                disconnects.RemoveAt(disconnects.Count - 1);
        }
    }

    private void Parse()
    {
        if (_stormMpqArchive is null)
            return;

        using MpqHeroesArchive stormMpqArchive = _stormMpqArchive;

        ParseReplayHeader();

        if (ReplayBuild < 32455)
        {
            _parseStatus = StormReplayParseStatus.PreAlphaWipe;
            return;
        }

        ArrayPool<byte> pool = ArrayPool<byte>.Shared;

        ParseMpqFile(pool, ReplayDetails.FileName, ReplayDetails.Parse);

        if (Timestamp == DateTime.MinValue)
        {
            // Uncommon issue when parsing replay.details
            return;
        }
        else if (Timestamp < new DateTime(2014, 10, 6, 0, 0, 0, DateTimeKind.Utc))
        {
            // Technical Alpha replays
            return;
        }

        ParseMpqFile(pool, ReplayInitData.FileName, ReplayInitData.Parse);
        ParseMpqFile(pool, ReplayAttributeEvents.FileName, ReplayAttributeEvents.Parse);
        ParseReplayServerBattlelobby(pool);

        if (_parseOptions.ShouldParseGameEvents)
            ParseMpqFile(pool, ReplayGameEvents.FileName, ReplayGameEvents.Parse);

        if (_parseOptions.ShouldParseTrackerEvents)
            ParseMpqFile(pool, ReplayTrackerEvents.FileName, ReplayTrackerEvents.Parse);

        if (_parseOptions.ShouldParseMessageEvents)
            ParseMpqFile(pool, ReplayMessageEvents.FileName, ReplayMessageEvents.Parse);

        ValidateResult();

        FinalPlayerData();
    }

    private void ParseMpqFile(ArrayPool<byte> pool, string fileName, MpqFileParser parser)
    {
        MpqHeroesArchiveEntry entry = _stormMpqArchive!.GetEntry(fileName);
        int size = (int)entry.FileSize;
        byte[] poolBuffer = pool.Rent(size);

        try
        {
            Span<byte> buffer = poolBuffer.AsSpan(..size);
            _stormMpqArchive.DecompressEntry(entry, buffer);
            parser(this, buffer);
        }
        finally
        {
            pool.Return(poolBuffer);
        }
    }

    private void ParseReplayHeader()
    {
        Span<byte> headerBuffer = stackalloc byte[MpqHeroesArchive.HeaderSize];

        _stormMpqArchive!.GetHeaderBytes(headerBuffer);
        StormReplayHeader.Parse(this, headerBuffer);
    }

    private void ParseReplayServerBattlelobby(ArrayPool<byte> pool)
    {
        ParseMpqFile(pool, ReplayServerBattlelobby.FileName, static (replay, buffer) =>
        {
            StormReplayPregame replayPregame = new() { ReplayBuild = replay.ReplayBuild };
            ReplayServerBattlelobby.Parse(replayPregame, buffer);
            replayPregame.TransferTo(replay);
        });
    }

    private void ValidateResult()
    {
        if (PlayersCount == 1)
            _parseStatus = StormReplayParseStatus.TryMeMode;
        else if (Players.All(x => x is not null && !x.IsWinner) || ReplayLength.TotalSeconds < 45)
            _parseStatus = StormReplayParseStatus.Incomplete;
        else if (Timestamp == DateTime.MinValue)
            _parseStatus = StormReplayParseStatus.UnexpectedResult;
        else if (Timestamp < new DateTime(2014, 10, 6, 0, 0, 0, DateTimeKind.Utc))
            _parseStatus = StormReplayParseStatus.PreAlphaWipe;
        else if (!_parseOptions.AllowPTR && Players.Any(x => x is not null && x.ToonHandle?.Region >= 90))
            _parseStatus = StormReplayParseStatus.PTRRegion;
        else if (!(Players.Count(x => x is not null && x.IsWinner) == 5 && PlayersCount == 10 && StormGameMode.AllGameModes.HasFlag(GameMode)))
            _parseStatus = StormReplayParseStatus.UnexpectedResult;
        else
            _parseStatus = StormReplayParseStatus.Success;
    }
}