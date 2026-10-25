namespace Heroes.StormReplayParser.Player;

/// <summary>
/// Contains the properties for a player.
/// </summary>
public class StormPlayer
{
    private Func<int, ScoreResult>? _scoreResult;
    private int? _player;
#if NET10_0_OR_GREATER
#else
    private int? _accountLevel;
#endif

    /// <summary>
    /// Gets or sets the player's name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the player's toon handle.
    /// </summary>
    public ToonHandle? ToonHandle { get; set; } = null;

    /// <summary>
    /// Gets or sets the player's control type.
    /// </summary>
    public PlayerType PlayerType { get; set; } = PlayerType.Unknown;

    /// <summary>
    /// Gets or sets the player's hero information.
    /// </summary>
    public PlayerHero? PlayerHero { get; set; } = null;

    /// <summary>
    /// Gets or sets the player's loadout information.
    /// </summary>
    public PlayerLoadout PlayerLoadout { get; set; } = new PlayerLoadout();

    /// <summary>
    /// Gets the player's hero's mastery tier levels.
    /// </summary>
    public IReadOnlyList<HeroMasteryTier> HeroMasteryTiers => [.. HeroMasteryTiersByHeroAttributeIdInternal.Values];

    /// <summary>
    /// Gets the amount of hero mastery tiers.
    /// </summary>
    public int HeroMasteryTiersCount => HeroMasteryTiersByHeroAttributeIdInternal.Count;

    /// <summary>
    /// Gets or sets the player's team id.
    /// </summary>
    public StormTeam Team { get; set; } = StormTeam.Unknown;

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
    public bool? IsVoiceSilenced { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the player is Blizzard staff.
    /// </summary>
    public bool? IsBlizzardStaff { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the player is auto select or not.
    /// </summary>
    public bool IsAutoSelect { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the player has an active boost.
    /// </summary>
    public bool? HasActiveBoost { get; set; }

    /// <summary>
    /// Gets or sets the player's battletag which serves as the players display name.
    /// </summary>
    public string BattleTagName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the player's account level.
    /// </summary>
    public int? AccountLevel
    {
#if NET10_0_OR_GREATER
        get;
        set => field = value == 0 ? null : value;
#else
        get => _accountLevel;
        set => _accountLevel = value == 0 ? null : value;
#endif
    }

    /// <summary>
    /// Gets or sets the player's party value. Those in the same party have the same value.
    /// </summary>
    public long? PartyValue { get; set; } = null;

    /// <summary>
    /// Gets or sets a value indicating whether the player's platform is a mac.
    /// </summary>
    public bool IsPlatformMac { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the player is the owner of this replay.
    /// </summary>
    public bool IsReplayOwner { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the player chooses the bans for their team.
    /// <para>In ranked games it is the player with the highest MMR, which is currently not known.</para>
    /// <para>For custom games, it is the specific player chosen to select the bans.</para>
    /// </summary>
    public bool IsBanChooser { get; set; }

    /// <summary>
    /// Gets or sets the license of the player's selected portrait. This is the <c>License</c> value of the <c>CRewardPortrait</c> in the game data.
    /// <para>
    /// This will be <see langword="null"/> if the portrait does not have a license, which are mostly progression and achievement portraits, or if the player is an observer.
    /// For these license-less portraits, the <see cref="PortraitUnlockableId"/> can be used to determine the portrait's name.
    /// </para>
    /// </summary>
    public long? PortraitLicenseId { get; set; }

    /// <summary>
    /// Gets or sets the Battle.net unlockable id of the player's selected portrait.
    /// <para>
    /// This is the <c>m_unlockableId</c> of an entry in the <c>PORT</c> unlockable file (.unlk) of the Battle.net cache.
    /// The portrait's name is in a string table file (.s2ql), at the <c>&lt;e&gt;</c> element whose id is the entry's <c>m_name.m_id</c>.
    /// </para>
    /// </summary>
    public long? PortraitUnlockableId { get; set; }

    /// <summary>
    /// Gets or sets the Battle.net unlockable id of the player's selected portrait border, such as the basic border or the Storm border.
    /// <para>
    /// This is the <c>m_unlockableId</c> of an entry in the <c>POVT</c> unlockable file (.unlk) of the Battle.net cache.
    /// The portrait's name is in a string table file (.s2ql), at the <c>&lt;e&gt;</c> element whose id is the entry's <c>m_name.m_id</c>.
    /// </para>
    /// </summary>
    public long? PortraitBorderUnlockableId { get; set; }

    /// <summary>
    /// Gets or sets the computer player difficulty.
    /// </summary>
    public ComputerDifficulty ComputerDifficulty { get; set; } = ComputerDifficulty.Unknown;

    /// <summary>
    /// Gets the player's score result.
    /// </summary>
    public ScoreResult? ScoreResult => _scoreResult?.Invoke(_player!.Value) ?? null;

    /// <summary>
    /// Gets the match awards earned.
    /// </summary>
    public IReadOnlyList<MatchAwardType>? MatchAwards => ScoreResult?.MatchAwards.ToList();

    /// <summary>
    /// Gets the player's selected talents.
    /// </summary>
    public IReadOnlyList<HeroTalent> Talents => TalentsInternal.AsReadOnly();

    /// <summary>
    /// Gets the player's times when they left and rejoined the game.
    /// </summary>
    public IReadOnlyList<PlayerDisconnect> PlayerDisconnects => PlayerDisconnectsInternal.AsReadOnly();

    /// <summary>
    /// Gets the amount of match awards.
    /// </summary>
    public int? MatchAwardsCount => ScoreResult?.MatchAwards.Count;

    /// <summary>
    /// Gets a collection of owned license ids.
    /// <para>
    /// This is used to determine if the player owns a specific hero, skin, mount, announcer, or other content.
    /// </para>
    /// </summary>
    public IReadOnlyList<long> OwnedLicenseIds => OwnedLicenseIdsInternal.AsReadOnly();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal Dictionary<string, HeroMasteryTier> HeroMasteryTiersByHeroAttributeIdInternal { get; set; } = new(StringComparer.Ordinal);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal List<HeroTalent> TalentsInternal { get; set; } = new(7);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal List<PlayerDisconnect> PlayerDisconnectsInternal { get; set; } = [];

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal int TalentSetCount { get; set; } = 0;

    internal TimeSpan LastCameraUpdateEvent { get; set; }

    internal uint? WorkingSetSlotId { get; set; }

    // from replay.initData, m_rewards, the reward tag values followed by the Battle.net unlockable id of each owned license
    internal uint[] RewardHashes { get; set; } = [];

    // from replay.server.battlelobby, license ids
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal List<long> OwnedLicenseIdsInternal { get; set; } = [];

    internal int? RewardTagsCount { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        if (PlayerHero is not null)
            return $"[{Name}-{PlayerHero}]-{PlayerType}-{ToonHandle}";
        else
            return $"[{Name}-{PlayerType}-{ToonHandle}";
    }

    internal void SetScoreResult(int player, Func<int, ScoreResult> scoreResult)
    {
        _player = player;
        _scoreResult = scoreResult;
    }
}
