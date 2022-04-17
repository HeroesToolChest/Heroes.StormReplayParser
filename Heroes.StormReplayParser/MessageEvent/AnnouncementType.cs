namespace Heroes.StormReplayParser.MessageEvent;

/// <summary>
/// Specifies the announcemnt type.
/// </summary>
/// <remarks>m_announcement.</remarks>
public enum AnnouncementType
{
    /// <summary>
    /// Indicates a type of none.
    /// </summary>
    None = 0,

    /// <summary>
    /// Indicates a type of ability.
    /// </summary>
    Ability = 1,

    /// <summary>
    /// Indicates a type of behavior.
    /// </summary>
    Behavior = 2,

    /// <summary>
    /// Indicates a type of vital.
    /// </summary>
    Vitals = 3,
}
