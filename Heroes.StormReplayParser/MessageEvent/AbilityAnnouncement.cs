namespace Heroes.StormReplayParser.MessageEvent;

/// <summary>
/// Contains the information for an ability announcement.
/// </summary>
public struct AbilityAnnouncement : IEquatable<AbilityAnnouncement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AbilityAnnouncement"/> struct.
    /// </summary>
    /// <param name="abilityIndex">The ability index.</param>
    /// <param name="abilityLink">The ability link.</param>
    /// <param name="buttonLink">The button link.</param>
    public AbilityAnnouncement(int abilityIndex, int abilityLink, int buttonLink)
    {
        AbilityIndex = abilityIndex;
        AbilityLink = abilityLink;
        ButtonLink = buttonLink;
    }

    /// <summary>
    /// Gets the ability index.
    /// </summary>
    public int AbilityIndex { get; }

    /// <summary>
    /// Gets the ability link number.
    /// </summary>
    public int AbilityLink { get; }

    /// <summary>
    /// Gets the button link number.
    /// </summary>
    public int ButtonLink { get; }

    /// <summary>
    /// Compares for equality.
    /// </summary>
    /// <param name="left">The object to the left hand side of the operator.</param>
    /// <param name="right">The object to the right hand side of the operator.</param>
    /// <returns>The value indicating the result of the comparison.</returns>
    public static bool operator ==(AbilityAnnouncement left, AbilityAnnouncement right) => left.Equals(right);

    /// <summary>
    /// Compare for inequality.
    /// </summary>
    /// <param name="left">The object to the left hand side of the operator.</param>
    /// <param name="right">The object to the right hand side of the operator.</param>
    /// <returns>The value indicating the result of the comparison.</returns>
    public static bool operator !=(AbilityAnnouncement left, AbilityAnnouncement right) => !(left == right);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (!(obj is AbilityAnnouncement))
            return false;

        return Equals((AbilityAnnouncement)obj);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(AbilityIndex, AbilityLink, ButtonLink);
    }

    /// <inheritdoc/>
    public bool Equals(AbilityAnnouncement other)
    {
        if (AbilityIndex == other.AbilityIndex &&
            AbilityLink == other.AbilityLink &&
            ButtonLink == other.ButtonLink)
            return true;

        return false;
    }
}
