﻿namespace Heroes.StormReplayParser;

/// <summary>
/// Contains the information to parse a Heroes of the Storm replay.
/// </summary>
public partial class StormReplay
{
    private static StormReplayParseStatus _stormReplayParseResult = StormReplayParseStatus.Incomplete;
    private static StormParseException? _failedReplayException = null;

    private readonly string _fileName;
    private readonly ParseOptions _parseOptions;
    private readonly MpqHeroesArchive _stormMpqArchive;

    private StormReplay(string fileName, ParseOptions parseOptions)
    {
        _fileName = fileName;
        _parseOptions = parseOptions;

        _stormMpqArchive = MpqHeroesFile.Open(_fileName);
    }

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
        TimeSpan latestCameraUpdateEvent = stormReplay.ClientListByUserID.Where(x => x is not null).Max(x => x!.LastCameraUpdateEvent);

        // remove the occurrence where the players leaves at the end of the match
        foreach (StormPlayer? player in stormReplay.ClientListByUserID)
        {
            if (player is null)
                continue;

            PlayerDisconnect? lastOccurrence = player.PlayerDisconnectsInternal.LastOrDefault();
            if (lastOccurrence is not null && lastOccurrence.From > latestCameraUpdateEvent)
                player.PlayerDisconnectsInternal.Remove(lastOccurrence);
        }
    }

    private void Parse(StormReplay stormReplay)
    {
        using MpqHeroesArchive stormMpqArchive = _stormMpqArchive;

        ParseReplayHeader(stormReplay);

        if (stormReplay.ReplayBuild < 32455)
        {
            _stormReplayParseResult = StormReplayParseStatus.PreAlphaWipe;
            return;
        }

        ArrayPool<byte> pool = ArrayPool<byte>.Shared;

        ParseReplayDetails(stormReplay, pool);

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

        ParseReplayInit(stormReplay, pool);
        ParseReplayAttributeEvents(stormReplay, pool);
        ParseReplayServerBattlelobby(stormReplay, pool);

        if (_parseOptions.ShouldParseGameEvents)
            ParseReplayGameEvents(stormReplay, pool);

        if (_parseOptions.ShouldParseTrackerEvents)
            ParseReplayTrackerEvents(stormReplay, pool);

        if (_parseOptions.ShouldParseMessageEvents)
            ParseReplayMessageEvents(stormReplay, pool);

        ValidateResult(stormReplay);

        FinalPlayerData(stormReplay);
    }

    private void ParseReplayHeader(StormReplay stormReplay)
    {
        Span<byte> headerBuffer = stackalloc byte[MpqHeroesArchive.HeaderSize];

        _stormMpqArchive.GetHeaderBytes(headerBuffer);
        StormReplayHeader.Parse(stormReplay, headerBuffer);
    }

    private void ParseReplayDetails(StormReplay stormReplay, ArrayPool<byte> pool)
    {
        MpqHeroesArchiveEntry entry = _stormMpqArchive.GetEntry(ReplayDetails.FileName);

        int size = (int)entry.FileSize;
        byte[] poolBuffer = pool.Rent(size);

        try
        {
            Span<byte> buffer = poolBuffer.AsSpan(..size);

            _stormMpqArchive.DecompressEntry(entry, buffer);
            ReplayDetails.Parse(stormReplay, buffer);
        }
        finally
        {
            pool.Return(poolBuffer);
        }
    }

    private void ParseReplayInit(StormReplay stormReplay, ArrayPool<byte> pool)
    {
        MpqHeroesArchiveEntry entry = _stormMpqArchive.GetEntry(ReplayInitData.FileName);

        int size = (int)entry.FileSize;
        byte[] poolBuffer = pool.Rent(size);

        try
        {
            Span<byte> buffer = poolBuffer.AsSpan(..size);

            _stormMpqArchive.DecompressEntry(entry, buffer);
            ReplayInitData.Parse(stormReplay, buffer);
        }
        finally
        {
            pool.Return(poolBuffer);
        }
    }

    private void ParseReplayAttributeEvents(StormReplay stormReplay, ArrayPool<byte> pool)
    {
        MpqHeroesArchiveEntry entry = _stormMpqArchive.GetEntry(ReplayAttributeEvents.FileName);

        int size = (int)entry.FileSize;
        byte[] poolBuffer = pool.Rent(size);

        try
        {
            Span<byte> buffer = poolBuffer.AsSpan(..size);

            _stormMpqArchive.DecompressEntry(entry, buffer);
            ReplayAttributeEvents.Parse(stormReplay, buffer);
        }
        finally
        {
            pool.Return(poolBuffer);
        }
    }

    private void ParseReplayTrackerEvents(StormReplay stormReplay, ArrayPool<byte> pool)
    {
        MpqHeroesArchiveEntry entry = _stormMpqArchive.GetEntry(ReplayTrackerEvents.FileName);

        int size = (int)entry.FileSize;
        byte[] poolBuffer = pool.Rent(size);

        try
        {
            Span<byte> buffer = poolBuffer.AsSpan(..size);

            _stormMpqArchive.DecompressEntry(entry, buffer);
            ReplayTrackerEvents.Parse(stormReplay, buffer);
        }
        finally
        {
            pool.Return(poolBuffer);
        }
    }

    private void ParseReplayMessageEvents(StormReplay stormReplay, ArrayPool<byte> pool)
    {
        MpqHeroesArchiveEntry entry = _stormMpqArchive.GetEntry(ReplayMessageEvents.FileName);

        int size = (int)entry.FileSize;
        byte[] poolBuffer = pool.Rent(size);

        try
        {
            Span<byte> buffer = poolBuffer.AsSpan(..size);

            _stormMpqArchive.DecompressEntry(entry, buffer);
            ReplayMessageEvents.Parse(stormReplay, buffer);
        }
        finally
        {
            pool.Return(poolBuffer);
        }
    }

    private void ParseReplayGameEvents(StormReplay stormReplay, ArrayPool<byte> pool)
    {
        MpqHeroesArchiveEntry entry = _stormMpqArchive.GetEntry(ReplayGameEvents.FileName);

        int size = (int)entry.FileSize;
        byte[] poolBuffer = pool.Rent(size);

        try
        {
            Span<byte> buffer = poolBuffer.AsSpan(..size);

            _stormMpqArchive.DecompressEntry(entry, buffer);
            ReplayGameEvents.Parse(stormReplay, buffer);
        }
        finally
        {
            pool.Return(poolBuffer);
        }
    }

    private void ParseReplayServerBattlelobby(StormReplay stormReplay, ArrayPool<byte> pool)
    {
        MpqHeroesArchiveEntry entry = _stormMpqArchive.GetEntry(ReplayServerBattlelobby.FileName);

        int size = (int)entry.FileSize;
        byte[] poolBuffer = pool.Rent(size);

        try
        {
            Span<byte> buffer = poolBuffer.AsSpan(..size);

            _stormMpqArchive.DecompressEntry(entry, buffer);

            StormReplayPregame replayPregame = new()
            {
                ReplayBuild = stormReplay.ReplayBuild,
            };

            ReplayServerBattlelobby.Parse(replayPregame, buffer);

            replayPregame.TransferTo(stormReplay);
        }
        finally
        {
            pool.Return(poolBuffer);
        }
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
