using Heroes.StormReplayParser.Decoders;
using System;

namespace Heroes.StormReplayParser
{
    /// <summary>
    /// Contains the properties for a tracker event.
    /// </summary>
    public struct StormTrackerEvent : IEquatable<StormTrackerEvent>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StormTrackerEvent"/> struct.
        /// </summary>
        /// <param name="trackerEventType">The event type of the tracker.</param>
        /// <param name="timestamp">The time the event took place.</param>
        /// <param name="versionedDecoder">Data associated with the event.</param>
        public StormTrackerEvent(StormTrackerEventType trackerEventType, TimeSpan timestamp, VersionedDecoder? versionedDecoder = null)
        {
            TrackerEventType = trackerEventType;
            Timestamp = timestamp;
            VersionedDecoder = versionedDecoder;
        }

        /// <summary>
        /// Gets the type of the tracker event.
        /// </summary>
        public StormTrackerEventType TrackerEventType { get; }

        /// <summary>
        /// Gets the time that the event took place.
        /// </summary>
        public TimeSpan Timestamp { get; }

        /// <summary>
        /// Gets the version decoder to obtain the data.
        /// </summary>
        public VersionedDecoder? VersionedDecoder { get; }

        /// <summary>
        /// Compares the <paramref name="left"/> value to the <paramref name="right"/> value and determines if they are equal.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><see langword="true"/> if the <paramref name="left"/> value is equal to the <paramref name="right"/> value; otherwise <see langword="false"/>.</returns>
        public static bool operator ==(StormTrackerEvent? left, StormTrackerEvent? right)
        {
            if (left is null)
                return right is null;
            return left.Equals(right);
        }

        /// <summary>
        /// Compares the <paramref name="left"/> value to the <paramref name="right"/> value and determines if they are not equal.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><see langword="true"/> if the <paramref name="left"/> value is not equal to the <paramref name="right"/> value; otherwise <see langword="false"/>.</returns>
        public static bool operator !=(StormTrackerEvent? left, StormTrackerEvent? right)
        {
            return !(left == right);
        }

        /// <inheritdoc/>
        public override string ToString() => $"[{Timestamp}] {TrackerEventType}: {VersionedDecoder}";

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;

            if (obj is not StormTrackerEvent trackerEvent)
                return false;
            else
                return Equals(trackerEvent);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Timestamp, TrackerEventType, VersionedDecoder?.ToString());
        }

        /// <inheritdoc/>
        public bool Equals(StormTrackerEvent other)
        {
            return Timestamp == other.Timestamp &&
                TrackerEventType == other.TrackerEventType &&
                VersionedDecoder?.ToString() == other.VersionedDecoder?.ToString();
        }
    }
}
