using System;

namespace Heroes.StormReplayParser.Replay
{
    /// <summary>
    /// Contains the properties for the replay version.
    /// </summary>
    public class StormReplayVersion
    {
        /// <summary>
        /// Gets or sets the first number.
        /// </summary>
        public int Major { get; set; }

        /// <summary>
        /// Gets or sets the second number.
        /// </summary>
        public int Minor { get; set; }

        /// <summary>
        /// Gets or sets the third number.
        /// </summary>
        public int Revision { get; set; }

        /// <summary>
        /// Gets or sets the fourth number.
        /// </summary>
        public int Build { get; set; }

        /// <summary>
        /// Gets or sets the fifth number.
        /// </summary>
        public int BaseBuild { get; set; }

        /// <inheritdoc/>
        public override string? ToString()
        {
            return $"{Major}.{Minor}.{Revision}.{Build}";
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (!(obj is StormReplayVersion item))
                return false;

            return item.BaseBuild == BaseBuild && item.Build == Build && item.Revision == Revision && item.Minor == Minor && item.Major == Major;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Major, Minor, Revision, Build, BaseBuild);
        }
    }
}
