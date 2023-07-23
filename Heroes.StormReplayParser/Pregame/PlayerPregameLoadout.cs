﻿namespace Heroes.StormReplayParser.Pregame;

public class PlayerPregameLoadout
{
    /// <summary>
    /// Gets or sets the player's skin / skin tint attribute id.
    /// </summary>
    public string SkinAndSkinTintAttributeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the player's mount / mount tint attribute id.
    /// </summary>
    public string MountAndMountTintAttributeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the player's banner attribute id.
    /// </summary>
    public string BannerAttributeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the player's Spray attribute id.
    /// </summary>
    public string SprayAttributeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the player's announcer pack attribute id.
    /// </summary>
    public string AnnouncerPackAttributeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the player's voice line attribute id.
    /// </summary>
    public string VoiceLineAttributeId { get; set; } = string.Empty;
}
