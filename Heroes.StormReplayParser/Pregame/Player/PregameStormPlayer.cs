namespace Heroes.StormReplayParser.Pregame.Player;

/// <summary>
/// Contains the properties for a player during the pregame.
/// </summary>
public class PregameStormPlayer
{
#if NET10_0_OR_GREATER
#else
    private int? _accountLevel = null;
#endif

    /// <summary>
    /// Gets the player's name.
    /// <para>See <see cref="BattleTagName"/> for full battletag name.</para>
    /// </summary>
    public string Name
    {
        get
        {
            int index = BattleTagName.IndexOf('#');

            if (index >= 0)
                return BattleTagName[..index];
            else
                return string.Empty;
        }
    }

    /// <summary>
    /// Gets or sets the player's toon handle.
    /// </summary>
    public ToonHandle? ToonHandle { get; set; } = null;

    /// <summary>
    /// Gets or sets the slot type.
    /// </summary>
    public PlayerSlotType PlayerSlotType { get; set; } = PlayerSlotType.Unknown;

    /// <summary>
    /// Gets or sets the player's control type.
    /// </summary>
    public PlayerType PlayerType { get; set; } = PlayerType.Unknown;

    /// <summary>
    /// Gets or sets the player's team. Observers are <see cref="StormTeam.Observer"/>.
    /// </summary>
    public StormTeam Team { get; set; } = StormTeam.Unknown;

    /// <summary>
    /// Gets or sets a value indicating whether the player chooses the bans for their team.
    /// <para>In ranked games, this will always be <see langword="false"/>, as it is not known who the specific player is.</para>
    /// </summary>
    public bool IsBanChooser { get; set; }

    /// <summary>
    /// Gets or sets the player's hero information.
    /// </summary>
    public PregamePlayerHero? PlayerHero { get; set; } = null;

    /// <summary>
    /// Gets or sets the player's loadout information.
    /// </summary>
    public PregamePlayerLoadout PlayerLoadout { get; set; } = new PregamePlayerLoadout();

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
    /// Gets or sets the computer player difficulty.
    /// </summary>
    public ComputerDifficulty ComputerDifficulty { get; set; } = ComputerDifficulty.Unknown;

    /// <summary>
    /// Gets or sets a value indicating whether the player is the owner of this replay.
    /// </summary>
    public bool IsReplayOwner { get; set; }

    /// <summary>
    /// Gets or sets the Battle.net unlockable id of the player's selected portrait.
    /// <para>
    /// This is the <c>m_unlockableId</c> of an entry in the <c>PORT</c> unlockable file (.unlk) of the Battle.net cache.
    /// The portrait's name is in a string table file (.s2ql), at the <c>&lt;e&gt;</c> element whose id is the entry's <c>m_name.m_id</c>.
    /// </para>
    /// </summary>
    public long? PortraitUnlockableId { get; set; }

    /// <summary>
    /// Gets or sets the Battle.net unlockable id of the player's selected portrait border, such as the Basic border or the Storm border.
    /// <para>
    /// This is the <c>m_unlockableId</c> of an entry in the <c>POVT</c> unlockable file (.unlk) of the Battle.net cache.
    /// The portrait's name is in a string table file (.s2ql), at the <c>&lt;e&gt;</c> element whose id is the entry's <c>m_name.m_id</c>.
    /// </para>
    /// </summary>
    public long? PortraitBorderUnlockableId { get; set; }

    /// <summary>
    /// Gets the player's mastery taunt tier for each hero that has at least tier 1.
    /// </summary>
    public IReadOnlyList<HeroMasteryTier> HeroMasteryTiers => [.. HeroMasteryTiersByHeroAttributeIdInternal.Values];

    /// <summary>
    /// Gets the amount of hero taunt mastery tiers.
    /// </summary>
    public int HeroMasteryTiersCount => HeroMasteryTiersByHeroAttributeIdInternal.Count;

    /// <summary>
    /// Gets a collection of the player's license ids. The license ids are the <c>License</c> values of the <c>CReward</c> type entries in the game data.
    /// <para>
    /// A license is for a hero, skin, mount, announcer, or other content. These are not only the player's owned content, but also the free-to-play
    /// heroes (including the ones that require a higher account level).
    /// </para>
    /// </summary>
    public IReadOnlyList<long> LicenseIds => LicenseIdsInternal.AsReadOnly();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal Dictionary<string, HeroMasteryTier> HeroMasteryTiersByHeroAttributeIdInternal { get; } = new(StringComparer.Ordinal);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal Dictionary<ReplayAttributeEventType, string> PlayerTeamValueByAttributeEventType { get; } = [];

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal List<long> LicenseIdsInternal { get; set; } = [];

    internal int? RewardTagsCount { get; set; }
}
