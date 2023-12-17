namespace Heroes.StormReplayParser.Player;

/// <summary>
/// Contains the properties for a player.
/// </summary>
public class StormPlayer
{
    private Func<int, ScoreResult>? _scoreResult;
    private int? _player;
    private int? _accountLevel = null;

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
    public IReadOnlyList<HeroMasteryTier> HeroMasteryTiers => HeroMasteryTiersInternal;

    /// <summary>
    /// Gets the amount of hero mastery tiers.
    /// </summary>
    public int HeroMasteryTiersCount => HeroMasteryTiersInternal.Count;

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
        get => _accountLevel;
        set => _accountLevel = value == 0 ? null : value;
    }

    /// <summary>
    /// Gets or sets the player's party value. Those in the same party have the same value.
    /// </summary>
    public long? PartyValue { get; set; } = null;

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
    public IReadOnlyList<HeroTalent> Talents => TalentsInternal;

    /// <summary>
    /// Gets the amount of match awards.
    /// </summary>
    public int? MatchAwardsCount => ScoreResult?.MatchAwards.Count;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal List<HeroMasteryTier> HeroMasteryTiersInternal { get; set; } = new List<HeroMasteryTier>();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal List<HeroTalent> TalentsInternal { get; set; } = new List<HeroTalent>(7);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal int TalentSetCount { get; set; } = 0;

    internal int? WorkingSetSlotId { get; set; }

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
