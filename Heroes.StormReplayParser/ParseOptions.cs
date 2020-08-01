namespace Heroes.StormReplayParser
{
    /// <summary>
    /// Contains the parsing options.
    /// </summary>
    public class ParseOptions
    {
        /// <summary>
        /// Gets the default parse options. Disallows ptr and allows parsing of tracker, game, and message events.
        /// </summary>
        public static ParseOptions DefaultParsing => new ParseOptions();

        /// <summary>
        /// Gets the minimal parse options. No parsing of tracker, game, or message events.
        /// </summary>
        public static ParseOptions MinimalParsing => new ParseOptions()
        {
            ShouldTrackerEvents = false,
            ShouldGameEvents = false,
            ShouldParseMessageEvents = false,
        };

        /// <summary>
        /// Gets or sets a value indicating whether ptr replays should be parsed.
        /// </summary>
        public bool AllowPTR { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whetherthe tracker events should be parsed.
        /// </summary>
        public bool ShouldTrackerEvents { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whetherthe game events should be parsed.
        /// </summary>
        public bool ShouldGameEvents { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether message events should be parsed.
        /// </summary>
        public bool ShouldParseMessageEvents { get; set; } = true;
    }
}
