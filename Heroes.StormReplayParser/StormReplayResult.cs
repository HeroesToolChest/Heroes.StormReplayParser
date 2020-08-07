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
        /// <param name="fileName">The file name of the replay file.</param>
        /// <param name="exception">The exception, if any.</param>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> is <see langword="null"/> or emtpy.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> or <paramref name="stormReplay"/> is <see langword="null"/>.</exception>
        public StormReplayResult(StormReplay stormReplay, StormReplayParseStatus stormReplayParseStatus, string fileName, Exception? exception = null)
        {
            Replay = stormReplay ?? throw new ArgumentNullException(nameof(stormReplay));
            Status = stormReplayParseStatus;
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
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

        /// <summary>
        /// Gets the file name (includes the path).
        /// </summary>
        public string FileName { get; }
    }
}
