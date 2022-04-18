namespace Heroes.StormReplayParser;

/// <summary>
/// Contains the properties for point coordinates.
/// </summary>
public struct Point : IEquatable<Point>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Point"/> struct.
    /// </summary>
    /// <param name="x">The X-coordinate.</param>
    /// <param name="y">The Y-coordinate.</param>
    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Gets the X coordinate.
    /// </summary>
    public double X { get; }

    /// <summary>
    /// Gets the Y coordinate.
    /// </summary>
    public double Y { get; }

    /// <summary>
    /// Compares for equality.
    /// </summary>
    /// <param name="left">The object to the left hand side of the operator.</param>
    /// <param name="right">The object to the right hand side of the operator.</param>
    /// <returns>The value indicating the result of the comparison.</returns>
    public static bool operator ==(Point left, Point right) => left.Equals(right);

    /// <summary>
    /// Compare for inequality.
    /// </summary>
    /// <param name="left">The object to the left hand side of the operator.</param>
    /// <param name="right">The object to the right hand side of the operator.</param>
    /// <returns>The value indicating the result of the comparison.</returns>
    public static bool operator !=(Point left, Point right) => !(left == right);

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{{{X}, {Y}}}";
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not Point)
            return false;

        return Equals((Point)obj);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return $"{X}, {Y}".GetHashCode();
    }

    /// <inheritdoc/>
    public bool Equals(Point other)
    {
        if (X != other.X)
            return false;

        return Y == other.Y;
    }
}
