using Heroes.StormReplayParser.Pregame.Player;

namespace Heroes.StormReplayParser;

public class StormReplayPregame
{
    /// <summary>
    /// Gets the list of all players connected to the game, using 'm_userId' as index.
    /// </summary>
    /// <remarks>Contains observers. No AI.</remarks>
    internal StormPregamePlayer[] ClientListByUserID { get; private set; } = new StormPregamePlayer[16];

    public int ReplayBuild { get; set; } = 9999999;

    /// <summary>
    /// Gets or sets the random value.
    /// </summary>
    public long RandomValue { get; set; }

    /// <summary>
    /// Gets or sets the game mode.
    /// </summary>
    public StormGameMode GameMode { get; set; } = StormGameMode.TryMe;

    /// <summary>
    /// Gets a collection of disabled heroes as attributeIds.
    /// </summary>
    public IReadOnlyCollection<string> DisabledHeroes => DisabledHeroAttributeIdList;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal List<string> DisabledHeroAttributeIdList { get; private set; } = new List<string>();
}
