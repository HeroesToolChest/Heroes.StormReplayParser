namespace Heroes.StormReplayParser;

/// <summary>
/// Contains the information to parse a Heroes of the Storm replay.
/// </summary>
public partial class StormReplay
{
    private static StormReplayParseStatus _stormReplayParseResult = StormReplayParseStatus.Incomplete;
    private static StormParseException? _failedReplayException = null;

    private readonly string _fileName;
    private readonly ParseOptions _parseOptions;
    private readonly MpqHeroesArchive? _stormMpqArchive;

    private StormReplay(string fileName, ParseOptions parseOptions)
    {
        _fileName = fileName;
        _parseOptions = parseOptions;

        try
        {
            _stormMpqArchive = MpqHeroesFile.Open(_fileName);
        }
        catch (Exception exception)
        {
            _failedReplayException = new StormParseException("An exception has occured during the parsing of the replay.", exception);
            _stormReplayParseResult = StormReplayParseStatus.Exception;
        }
    }

    private delegate void MpqFileParser(StormReplay replay, ReadOnlySpan<byte> source);

    /// <summary>
    /// Parses a .StormReplay file.
    /// </summary>
    /// <param name="fileName">The file name which may contain the path.</param>
    /// <param name="parseOptions">Sets the parsing options. If <see cref="ParseOptions.AllowPTR"/> is <see langword="false"/> the result status will be <see cref="StormReplayParseStatus.PTRRegion"/> if the replay is successfully parsed.</param>
    /// <returns>A <see cref="StormReplayResult"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is null.</exception>
    public static StormReplayResult Parse(string fileName, ParseOptions? parseOptions = null)
    {
        ArgumentNullException.ThrowIfNull(fileName);

        parseOptions ??= ParseOptions.DefaultParsing;

        StormReplay stormReplay = ParseStormReplay(fileName, parseOptions);

        return new StormReplayResult(stormReplay, _stormReplayParseResult, fileName, _failedReplayException);
    }

    private static StormReplay ParseStormReplay(string fileName, ParseOptions parseOptions)
    {
        StormReplay stormReplay = new(fileName, parseOptions);

        try
        {
            stormReplay.Parse(stormReplay);
        }
        catch (Exception exception)
        {
            _failedReplayException = new StormParseException("An exception has occured during the parsing of the replay.", exception);
            _stormReplayParseResult = StormReplayParseStatus.Exception;
        }

        return stormReplay;
    }

    private static void FinalPlayerData(StormReplay stormReplay)
    {
        TimeSpan latestCameraUpdateEvent = TimeSpan.MinValue;
        bool foundPlayer = false;

        foreach (StormPlayer? player in stormReplay.ClientListByUserID)
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
        foreach (StormPlayer? player in stormReplay.ClientListByUserID)
        {
            if (player is null)
                continue;

            List<PlayerDisconnect> disconnects = player.PlayerDisconnectsInternal;

            if (disconnects.Count > 0 && disconnects[^1].From > latestCameraUpdateEvent)
                disconnects.RemoveAt(disconnects.Count - 1);
        }
    }

    private void Parse(StormReplay stormReplay)
    {
        if (_stormMpqArchive is null)
            return;

        using MpqHeroesArchive stormMpqArchive = _stormMpqArchive;

        ParseReplayHeader(stormReplay);

        if (stormReplay.ReplayBuild < 32455)
        {
            _stormReplayParseResult = StormReplayParseStatus.PreAlphaWipe;
            return;
        }

        ArrayPool<byte> pool = ArrayPool<byte>.Shared;

        ParseMpqFile(stormReplay, pool, ReplayDetails.FileName, ReplayDetails.Parse);

        if (stormReplay.Timestamp == DateTime.MinValue)
        {
            // Uncommon issue when parsing replay.details
            return;
        }
        else if (stormReplay.Timestamp < new DateTime(2014, 10, 6, 0, 0, 0, DateTimeKind.Utc))
        {
            // Technical Alpha replays
            return;
        }

        ParseMpqFile(stormReplay, pool, ReplayInitData.FileName, ReplayInitData.Parse);
        ParseMpqFile(stormReplay, pool, ReplayAttributeEvents.FileName, ReplayAttributeEvents.Parse);
        ParseReplayServerBattlelobby(stormReplay, pool);

        if (_parseOptions.ShouldParseGameEvents)
            ParseMpqFile(stormReplay, pool, ReplayGameEvents.FileName, ReplayGameEvents.Parse);

        if (_parseOptions.ShouldParseTrackerEvents)
            ParseMpqFile(stormReplay, pool, ReplayTrackerEvents.FileName, ReplayTrackerEvents.Parse);

        if (_parseOptions.ShouldParseMessageEvents)
            ParseMpqFile(stormReplay, pool, ReplayMessageEvents.FileName, ReplayMessageEvents.Parse);

        ValidateResult(stormReplay);

        FinalPlayerData(stormReplay);
    }

    private void ParseMpqFile(StormReplay stormReplay, ArrayPool<byte> pool, string fileName, MpqFileParser parser)
    {
        MpqHeroesArchiveEntry entry = _stormMpqArchive!.GetEntry(fileName);
        int size = (int)entry.FileSize;
        byte[] poolBuffer = pool.Rent(size);

        try
        {
            Span<byte> buffer = poolBuffer.AsSpan(..size);
            _stormMpqArchive.DecompressEntry(entry, buffer);
            parser(stormReplay, buffer);
        }
        finally
        {
            pool.Return(poolBuffer);
        }
    }

    private void ParseReplayHeader(StormReplay stormReplay)
    {
        Span<byte> headerBuffer = stackalloc byte[MpqHeroesArchive.HeaderSize];

        _stormMpqArchive!.GetHeaderBytes(headerBuffer);
        StormReplayHeader.Parse(stormReplay, headerBuffer);
    }

    private void ParseReplayServerBattlelobby(StormReplay stormReplay, ArrayPool<byte> pool)
    {
        ParseMpqFile(stormReplay, pool, ReplayServerBattlelobby.FileName, (replay, buffer) =>
        {
            StormReplayPregame replayPregame = new() { ReplayBuild = replay.ReplayBuild };
            ReplayServerBattlelobby.Parse(replayPregame, buffer);
            replayPregame.TransferTo(replay);
        });
    }

    private void ValidateResult(StormReplay stormReplay)
    {
        if (stormReplay.PlayersCount == 1)
            _stormReplayParseResult = StormReplayParseStatus.TryMeMode;
        else if (stormReplay.Players.All(x => x is not null && !x.IsWinner) || stormReplay.ReplayLength.TotalSeconds < 45)
            _stormReplayParseResult = StormReplayParseStatus.Incomplete;
        else if (stormReplay.Timestamp == DateTime.MinValue)
            _stormReplayParseResult = StormReplayParseStatus.UnexpectedResult;
        else if (stormReplay.Timestamp < new DateTime(2014, 10, 6, 0, 0, 0, DateTimeKind.Utc))
            _stormReplayParseResult = StormReplayParseStatus.PreAlphaWipe;
        else if (!_parseOptions.AllowPTR && stormReplay.Players.Any(x => x is not null && x.ToonHandle?.Region >= 90))
            _stormReplayParseResult = StormReplayParseStatus.PTRRegion;
        else if (!(stormReplay.Players.Count(x => x is not null && x.IsWinner) == 5 && stormReplay.PlayersCount == 10 && StormGameMode.AllGameModes.HasFlag(stormReplay.GameMode)))
            _stormReplayParseResult = StormReplayParseStatus.UnexpectedResult;
        else
            _stormReplayParseResult = StormReplayParseStatus.Success;
    }
}
