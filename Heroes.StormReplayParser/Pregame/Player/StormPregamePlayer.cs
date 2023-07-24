namespace Heroes.StormReplayParser.Pregame.Player;

/// <summary>
/// Contains the properties for a player during the pregame.
/// </summary>
public class StormPregamePlayer
{
    private int? _accountLevel = null;

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
    /// Gets or sets the player's hero information.
    /// </summary>
    public PregamePlayerHero? PlayerHero { get; set; } = null;

    /// <summary>
    /// Gets or sets the player's loadout information.
    /// </summary>
    public PlayerPregameLoadout PlayerLoadout { get; set; } = new PlayerPregameLoadout();

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
        get => _accountLevel;
        set => _accountLevel = value == 0 ? null : value;
    }

    /// <summary>
    /// Gets or sets the player's party value. Those in the same party have the same value.
    /// </summary>
    public long? PartyValue { get; set; } = null;
}
