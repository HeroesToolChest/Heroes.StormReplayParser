namespace Heroes.StormReplayParser.Replay;

/// <summary>
/// Indicates the read mode setting.
/// </summary>
public enum StormReadyMode
{
    /// <summary>
    /// Indicates an unknown ready mode.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Indicates a First Come First Serve mode.
    /// </summary>
    FCFS = 1,

    /// <summary>
    /// Indicates a predetermined mode.
    /// </summary>
    Predetermined = 2,
}
