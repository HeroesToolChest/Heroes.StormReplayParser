using System;
using System.Diagnostics.CodeAnalysis;

namespace Heroes.StormReplayParser.Replay
{
    /// <summary>
    /// Contains the properties for the replay version.
    /// </summary>
    public class StormReplayVersion : IComparable, IComparable<StormReplayVersion>, IEquatable<StormReplayVersion>
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

        /// <summary>
        /// Compares the <paramref name="left"/> value to the <paramref name="right"/> value and determines if they are equal.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><see langword="true"/> if the <paramref name="left"/> value is equal to the <paramref name="right"/> value; otherwise <see langword="false"/>.</returns>
        public static bool operator ==(StormReplayVersion? left, StormReplayVersion? right)
        {
            if (left is null)
                return right is null;
            return left.Equals(right);
        }

        /// <summary>
        /// Compares the <paramref name="left"/> value to the <paramref name="right"/> vlaue and determines if they are not equal.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><see langword="true"/> if the <paramref name="left"/> value is not equal to the <paramref name="right"/> value; otherwise <see langword="false"/>.</returns>
        public static bool operator !=(StormReplayVersion? left, StormReplayVersion? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Compares the <paramref name="left"/> value to the <paramref name="right"/> value and determines if the <paramref name="left"/> value is less than the <paramref name="right"/> value.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><see langword="true"/> if the <paramref name="left"/> value is less than the <paramref name="right"/> value; otherwise <see langword="false"/>.</returns>
        public static bool operator <(StormReplayVersion? left, StormReplayVersion? right)
        {
            return left is null ? right is object : left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Compares the <paramref name="left"/> value to the <paramref name="right"/> value and determines if the <paramref name="left"/> value is less than or equal to the <paramref name="right"/> value.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><see langword="true"/> if the <paramref name="left"/> value is less than or equal to the <paramref name="right"/> value; otherwise <see langword="false"/>.</returns>
        public static bool operator <=(StormReplayVersion? left, StormReplayVersion? right)
        {
            return left is null || left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Compares the <paramref name="left"/> value to the <paramref name="right"/> value and determines if the <paramref name="left"/> value is greater than the <paramref name="right"/> value.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><see langword="true"/> if the <paramref name="left"/> value is greater than the <paramref name="right"/> value; otherwise <see langword="false"/>.</returns>
        public static bool operator >(StormReplayVersion? left, StormReplayVersion? right)
        {
            return left is object && left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Compares the <paramref name="left"/> value to the <paramref name="right"/> value and determines if the <paramref name="left"/> value is greater than or equal to the <paramref name="right"/> value.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><see langword="true"/> if the <paramref name="left"/> value is greater than or equal to the <paramref name="right"/> value; otherwise <see langword="false"/>.</returns>
        public static bool operator >=(StormReplayVersion? left, StormReplayVersion? right)
        {
            return left is null ? right is null : left.CompareTo(right) >= 0;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Major}.{Minor}.{Revision}.{Build}";
        }

        /// <inheritdoc/>
        public int CompareTo([AllowNull] StormReplayVersion other)
        {
            if (other is null)
                return 1;

            int valueCompare = BaseBuild.CompareTo(other.BaseBuild);
            if (valueCompare != 0)
                return valueCompare;

            valueCompare = Build.CompareTo(other.Build);
            if (valueCompare != 0)
                return valueCompare;

            valueCompare = Revision.CompareTo(other.Revision);
            if (valueCompare != 0)
                return valueCompare;

            valueCompare = Minor.CompareTo(other.Minor);
            if (valueCompare != 0)
                return valueCompare;

            valueCompare = Major.CompareTo(other.Major);
            if (valueCompare != 0)
                return valueCompare;

            return 0;
        }

        /// <inheritdoc/>
        public int CompareTo(object? obj)
        {
            if (ReferenceEquals(this, obj))
                return 0;
            if (obj is null)
                return 1;

            if (obj is not StormReplayVersion stormReplayVersion)
                throw new ArgumentException($"{nameof(obj)} is not a {nameof(Replay.StormReplayVersion)}");
            else
                return CompareTo(stormReplayVersion);
        }

        /// <inheritdoc/>
        public bool Equals([AllowNull] StormReplayVersion other)
        {
            if (other is null)
                return false;

            return BaseBuild == other.BaseBuild &&
                Build == other.Build &&
                Revision == other.Revision &&
                Minor == other.Minor &&
                Major == other.Major;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj is null)
                return false;

            if (obj is not StormReplayVersion stormReplayVersion)
                return false;
            else
                return Equals(stormReplayVersion);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Major, Minor, Revision, Build, BaseBuild);
        }
    }
}
