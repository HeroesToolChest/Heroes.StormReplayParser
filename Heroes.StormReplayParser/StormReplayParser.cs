using Heroes.StormReplayParser.MpqFiles;
using Heroes.StormReplayParser.MpqHeroesTool;
using Heroes.StormReplayParser.Replay;
using System;
using System.Linq;

namespace Heroes.StormReplayParser
{
    /// <summary>
    /// Contains the information to parse a Heroes of the Storm replay.
    /// </summary>
    public class StormReplayParser
    {
        private static StormReplayParseStatus _stormReplayParseResult = StormReplayParseStatus.Incomplete;
        private static Exception? _failedReplayException = null;

        private readonly string _fileName;
        private readonly bool _allowPTRRegion;
        private readonly bool _parseBattleLobby;
        private readonly MpqHeroesArchive _stormMpqArchive;

        private readonly StormReplay _stormReplay = new StormReplay();

        private StormReplayParser(string fileName, bool allowPTRRegion, bool parseBattleLobby)
        {
            _fileName = fileName;
            _allowPTRRegion = allowPTRRegion;
            _parseBattleLobby = parseBattleLobby;

            _stormMpqArchive = MpqHeroesFile.Open(_fileName);
        }

        /// <summary>
        /// Parses a .StormReplay file.
        /// </summary>
        /// <param name="fileName">The file name which may contain the path.</param>
        /// <param name="allowPTRRegion">If false, the result status will be <see cref="StormReplayParseStatus.PTRRegion"/> if the replay is successfully parsed.</param>
        /// <param name="parseBattleLobby">If enabled, the battle lobby file will be parsed which gives more available data.</param>
        /// <returns>A <see cref="StormReplayResult"/>.</returns>
        public static StormReplayResult Parse(string fileName, bool allowPTRRegion = false, bool parseBattleLobby = false)
        {
            StormReplay stormReplay = ParseStormReplay(fileName, allowPTRRegion, parseBattleLobby);

            if (_failedReplayException != null)
                return new StormReplayResult(stormReplay, _stormReplayParseResult, _failedReplayException);
            else
                return new StormReplayResult(stormReplay, _stormReplayParseResult);
        }

        private static StormReplay ParseStormReplay(string fileName, bool allowPTRRegion, bool parseBattleLobby)
        {
            StormReplayParser stormReplayParser = new StormReplayParser(fileName, allowPTRRegion, parseBattleLobby);

            BitReader.ResetIndex();
            BitReader.EndianType = EndianType.BigEndian;

            try
            {
                stormReplayParser.Parse();
            }
            catch (Exception exception)
            {
                _failedReplayException = exception;
                _stormReplayParseResult = StormReplayParseStatus.Exception;
            }

            return stormReplayParser._stormReplay;
        }

        private void Parse()
        {
            _stormMpqArchive.AddListfileFileNames();

            StormReplayHeader.Parse(_stormReplay, _stormMpqArchive.GetHeaderBytes());

            if (_stormReplay.ReplayBuild < 32455)
            {
                _stormReplayParseResult = StormReplayParseStatus.PreAlphaWipe;
                return;
            }

            ReplayDetails.Parse(_stormReplay, _stormMpqArchive.OpenFile(ReplayDetails.FileName));

            if (_stormReplay.Players.Length != 10 || _stormReplay.Players.Count(i => i.IsWinner) != 5)
            {
                // Filter out 'Try Me' games, any games without 10 players, and incomplete games
                return;
            }
            else if (_stormReplay.Timestamp == DateTime.MinValue)
            {
                // Uncommon issue when parsing replay.details
                return;
            }
            else if (_stormReplay.Timestamp < new DateTime(2014, 10, 6, 0, 0, 0, DateTimeKind.Utc))
            {
                // Technical Alpha replays
                return;
            }

            ReplayInitData.Parse(_stormReplay, _stormMpqArchive.OpenFile(ReplayInitData.FileName));
            ReplayAttributeEvents.Parse(_stormReplay, _stormMpqArchive.OpenFile(ReplayAttributeEvents.FileName));
            ReplayTrackerEvents.Parse(_stormReplay, _stormMpqArchive.OpenFile(ReplayTrackerEvents.FileName));
            ReplayMessageEvents.Parse(_stormReplay, _stormMpqArchive.OpenFile(ReplayMessageEvents.FileName));

            if (_parseBattleLobby)
                ReplayServerBattlelobby.Parse(_stormReplay, _stormMpqArchive.OpenFile(ReplayServerBattlelobby.FileName));

            ValidateResult();

            _stormReplay.SetStormPlayerData();
        }

        private void ValidateResult()
        {
            if (_stormReplay.PlayersCount == 1)
                _stormReplayParseResult = StormReplayParseStatus.TryMeMode;
            else if (_stormReplay.Players.All(x => !x.IsWinner) || _stormReplay.ReplayLength.Minutes < 2)
                _stormReplayParseResult = StormReplayParseStatus.Incomplete;
            else if (_stormReplay.Timestamp == DateTime.MinValue)
                _stormReplayParseResult = StormReplayParseStatus.UnexpectedResult;
            else if (_stormReplay.Timestamp < new DateTime(2014, 10, 6, 0, 0, 0, DateTimeKind.Utc))
                _stormReplayParseResult = StormReplayParseStatus.PreAlphaWipe;
            else if (!_allowPTRRegion && _stormReplay.Players.Any(x => x.ToonHandle.Region >= 90))
                _stormReplayParseResult = StormReplayParseStatus.PTRRegion;
            else if (_stormReplay.Players.Count(x => x.IsWinner) != 5 || _stormReplay.PlayersCount != 10 || !GameMode.AllGameModes.HasFlag(_stormReplay.GameMode))
                _stormReplayParseResult = StormReplayParseStatus.UnexpectedResult;
            else
                _stormReplayParseResult = StormReplayParseStatus.Success;
        }
    }
}
