namespace Heroes.StormReplayParser
{
    /// <summary>
    /// Specifies the status of the parse attempt.
    /// </summary>
    public enum StormReplayParseStatus
    {
        /// <summary>
        /// Indicates an unknown parse.
        /// </summary>
        Unknonwn = -1,

        /// <summary>
        /// Indicates a successful parse.
        /// </summary>
        Success = 0,

        /// <summary>
        /// Indicates an incomplete parse.
        /// </summary>
        Incomplete = 2,

        /// <summary>
        /// Indicates the replay was a try me game.
        /// </summary>
        TryMeMode = 5,

        /// <summary>
        /// Indicates an unexpected result.
        /// </summary>
        UnexpectedResult = 9,

        /// <summary>
        /// Indicates an exception occured while parsing.
        /// </summary>
        Exception = 10,

        /// <summary>
        /// Indicates the replay was not found.
        /// </summary>
        FileNotFound = 11,

        /// <summary>
        /// Indicates the replay is an extremely old replay.
        /// </summary>
        PreAlphaWipe = 13,

        /// <summary>
        /// Indicates the replay size is too large.
        /// </summary>
        FileSizeTooLarge = 14,

        /// <summary>
        /// Indicates a successful parse and that the replay is from the PTR region.
        /// </summary>
        PTRRegion = 15,
    }
}
