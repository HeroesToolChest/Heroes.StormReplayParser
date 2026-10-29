namespace Heroes.StormReplayParser.Replay;

/// <summary>
/// Contains the properties for map information.
/// </summary>
public class StormMapInfo
{
    /// <summary>
    /// Gets or sets the map name. This is localized, recommended to use <see cref="MapId"/> instead.
    /// </summary>
    public string MapName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the map id name. Not all maps have this set. Sandbox maps could have this set to their respective non-sandbox map id.
    /// <para>This is from the map's stormmod <c>mapscript.galaxy</c> file.</para>
    /// <para>
    /// This requires <see cref="ParseOptions.ShouldParseTrackerEvents"/>. If it is <see langword="false"/>, this will be <see langword="null"/>.
    /// </para>
    /// </summary>
    public string? MapId { get; set; }

    /// <summary>
    /// Gets or sets the map size.
    /// </summary>
    public Point MapSize { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{MapId} - {MapName} {MapSize}";
    }
}
