namespace Heroes.StormReplayParser.MessageEvent;

/// <summary>
/// Specifices the type of vital.
/// </summary>
public enum VitalType
{
    /// <summary>
    /// Indicates a health vital.
    /// </summary>
    Health = 0,

    /// <summary>
    /// Indicates an energy vital.
    /// <para>Energy is <c>mana</c>, <c>fury</c>, <c>brew</c>, etc...</para>
    /// </summary>
    Energy = 2,
}
