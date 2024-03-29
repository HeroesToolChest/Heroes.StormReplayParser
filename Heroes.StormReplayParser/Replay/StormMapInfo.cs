﻿namespace Heroes.StormReplayParser.Replay;

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
