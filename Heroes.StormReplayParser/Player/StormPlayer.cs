using Heroes.StormReplayParser.Replay;
using System;
using System.Collections.Generic;

namespace Heroes.StormReplayParser.Player
{
    /// <summary>
    /// Contains the properties for a player.
    /// </summary>
    public class StormPlayer
    {
        private Func<int, ScoreResult>? _scoreResult;
        private int? _player;

        /// <summary>
        /// Gets or sets the player's name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the player's toon handle.
        /// </summary>
        public ToonHandle ToonHandle { get; set; } = new ToonHandle();

        /// <summary>
        /// Gets or sets the player's control type.
        /// </summary>
        public PlayerType PlayerType { get; set; } = PlayerType.Observer;

        /// <summary>
        /// Gets or sets the player's hero information.
        /// </summary>
        public PlayerHero PlayerHero { get; set; } = new PlayerHero();

        /// <summary>
        /// Gets or sets the player's loadout information.
        /// </summary>
        public PlayerLoadout PlayerLoadout { get; set; } = new PlayerLoadout();

        /// <summary>
        /// Gets the player's hero's mastery tier levels.
        /// </summary>
        public IEnumerable<HeroMasteryTier> HeroMasteryTiers => HeroMasteryTiersInternal;

        /// <summary>
        /// Gets the amount of hero mastery tiers.
        /// </summary>
        public int HeroMasteryTiersCount => HeroMasteryTiersInternal.Count;

        /// <summary>
        /// Gets or sets the player's team id.
        /// </summary>
        public StormTeam Team { get; set; } = StormTeam.Observer;

        /// <summary>
        /// Gets or sets the player's handicap.
        /// </summary>
        public int Handicap { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player won the game.
        /// </summary>
        public bool IsWinner { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the player has been given the silenced penalty.
        /// </summary>
        public bool IsSilenced { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the player has been given the voice silence penalty.
        /// </summary>
        public bool IsVoiceSilenced { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the player is Blizzard staff.
        /// </summary>
        public bool IsBlizzardStaff { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the player is auto select or not.
        /// </summary>
        public bool IsAutoSelect { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the player has an active boost.
        /// </summary>
        public bool HasActiveBoost { get; set; } = false;

        /// <summary>
        /// Gets or sets the player's battletag which serves as the players display name.
        /// </summary>
        public string BattleTag { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the player's TID that serves as a unique identifier that is associated with the <see cref="BattleTag"/>. May not always start with T:.
        /// </summary>
        public string BattleTID { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the player's account level.
        /// </summary>
        public int? AccountLevel { get; set; } = null;

        /// <summary>
        /// Gets or sets the player's party value. Those in the same party have the same value.
        /// </summary>
        public long? PartyValue { get; set; } = null;

        /// <summary>
        /// Gets or sets the computer player difficulty.
        /// </summary>
        public PlayerDifficulty PlayerDifficulty { get; set; } = PlayerDifficulty.Unknown;

        /// <summary>
        /// Gets the player's score result.
        /// </summary>
        public ScoreResult ScoreResult => _scoreResult?.Invoke(_player!.Value) ?? new ScoreResult();

        /// <summary>
        /// Gets the match awards earned.
        /// </summary>
        public IEnumerable<MatchAwardType> MatchAwards => ScoreResult.MatchAwards;

        /// <summary>
        /// Gets the amount of match awards.
        /// </summary>
        public int MatchAwardsCount => ScoreResult.MatchAwards.Count;

        internal List<HeroMasteryTier> HeroMasteryTiersInternal { get; set; } = new List<HeroMasteryTier>();

        internal int? WorkingSetSlotId { get; set; }

        /// <inheritdoc/>
        public override string? ToString()
        {
            return $"{Name}-{PlayerType}-{ToonHandle}";
        }

        internal void SetScoreResult(int player, Func<int, ScoreResult> scoreResult)
        {
            _player = player;
            _scoreResult = scoreResult;
        }
    }
}
