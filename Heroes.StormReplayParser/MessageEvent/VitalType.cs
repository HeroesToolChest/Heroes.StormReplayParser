namespace Heroes.StormReplayParser.MessageEvent
{
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
        /// </summary>
        /// <remarks>Energy is mana, fury, brew, etc...</remarks>
        Energy = 2,
    }
}
