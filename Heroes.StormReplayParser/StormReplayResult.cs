using Heroes.StormReplayParser.Replay;
using System;

namespace Heroes.StormReplayParser
{
    /// <summary>
    /// Represents the result of the storm replay parsing.
    /// </summary>
    public class StormReplayResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StormReplayResult"/> class.
        /// </summary>
        /// <param name="stormReplay">The parsed <see cref="StormReplay"/>.</param>
        /// <param name="stormReplayParseStatus">The <see cref="StormReplayParseStatus"/>.</param>
        public StormReplayResult(StormReplay stormReplay, StormReplayParseStatus stormReplayParseStatus)
        {
            Replay = stormReplay;
            Status = stormReplayParseStatus;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StormReplayResult"/> class.
        /// </summary>
        /// <param name="stormReplay">The parsed <see cref="StormReplay"/>.</param>
        /// <param name="stormReplayParseStatus">The <see cref="StormReplayParseStatus"/>.</param>
        /// <param name="exception">The exception, if any.</param>
        public StormReplayResult(StormReplay stormReplay, StormReplayParseStatus stormReplayParseStatus, Exception exception)
        {
            Replay = stormReplay;
            Status = stormReplayParseStatus;
            Exception = exception;
        }

        /// <summary>
        /// Gets the status of the parsed replay.
        /// </summary>
        public StormReplayParseStatus Status { get; }

        /// <summary>
        /// Gets the exception, if any, from the parsed replay.
        /// </summary>
        public Exception? Exception { get; } = null;

        /// <summary>
        /// Gets the parsed <see cref="StormReplay"/>.
        /// </summary>
        public StormReplay Replay { get; }
    }
}
