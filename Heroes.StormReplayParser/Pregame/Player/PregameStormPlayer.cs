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
    /// <para>In ranked games it is the player with the highest MMR, which is currently not known.</para>
    /// <para>For custom games, it is the specific player chosen to select the bans.</para>
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
    /// Gets the player's hero's mastery tier levels. Only heroes with a tier level are included.
    /// <para>Only parsed for build 89754 and later.</para>
    /// </summary>
    public IReadOnlyList<HeroMasteryTier> HeroMasteryTiers => [.. HeroMasteryTiersByHeroAttributeIdInternal.Values];

    /// <summary>
    /// Gets the amount of hero mastery tiers.
    /// </summary>
    public int HeroMasteryTiersCount => HeroMasteryTiersByHeroAttributeIdInternal.Count;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal Dictionary<string, HeroMasteryTier> HeroMasteryTiersByHeroAttributeIdInternal { get; } = new(StringComparer.Ordinal);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal Dictionary<ReplayAttributeEventType, string> PlayerTeamValueByAttributeEventType { get; } = [];
}
