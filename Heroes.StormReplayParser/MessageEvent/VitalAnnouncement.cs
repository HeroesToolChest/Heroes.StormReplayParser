namespace Heroes.StormReplayParser.MessageEvent;

/// <summary>
/// Contains the information for a vital announcment.
/// </summary>
public struct VitalAnnouncement : System.IEquatable<VitalAnnouncement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VitalAnnouncement"/> struct.
    /// </summary>
    /// <param name="vitalType">The type of vital.</param>
    public VitalAnnouncement(VitalType vitalType)
    {
        VitalType = vitalType;
    }

    /// <summary>
    /// Gets the type of vital.
    /// </summary>
    public VitalType VitalType { get; }

    /// <summary>
    /// Compares for equality.
    /// </summary>
    /// <param name="left">The object to the left hand side of the operator.</param>
    /// <param name="right">The object to the right hand side of the operator.</param>
    /// <returns>The value indicating the result of the comparison.</returns>
    public static bool operator ==(VitalAnnouncement left, VitalAnnouncement right) => left.Equals(right);

    /// <summary>
    /// Compare for inequality.
    /// </summary>
    /// <param name="left">The object to the left hand side of the operator.</param>
    /// <param name="right">The object to the right hand side of the operator.</param>
    /// <returns>The value indicating the result of the comparison.</returns>
    public static bool operator !=(VitalAnnouncement left, VitalAnnouncement right) => !(left == right);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not VitalAnnouncement)
            return false;

        return Equals((VitalAnnouncement)obj);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return (int)VitalType ^ 13;
    }

    /// <inheritdoc/>
    public bool Equals(VitalAnnouncement other)
    {
        return VitalType == other.VitalType;
    }
}
