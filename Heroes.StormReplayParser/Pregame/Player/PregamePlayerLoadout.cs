namespace Heroes.StormReplayParser.Pregame.Player;

/// <summary>
/// Contains a player's loadout during the pregame.
/// </summary>
public class PregamePlayerLoadout
{
    /// <summary>
    /// Gets or sets the player's skin attribute id of their selected hero.
    /// <para>This is the <c>AttributeId</c> value of the <c>CSkin</c> element in the game data.</para>
    /// </summary>
    public string SkinAndSkinTintAttributeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the player's mount of their selected hero.
    /// <para>This is the <c>id</c> value of the <c>CMount</c> element in the game data.</para>
    /// </summary>
    public string MountAndMountTintAttributeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the player's banner attribute id.
    /// <para>This is the <c>AttributeId</c> value of the <c>CBanner</c> element in the game data.</para>
    /// </summary>
    public string BannerAttributeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the player's Spray attribute id.
    /// <para>This is the <c>AttributeId</c> value of the <c>CSpray</c> element in the game data.</para>
    /// </summary>
    public string SprayAttributeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the player's announcer pack attribute id.
    /// <para>This is the <c>AttributeId</c> value of the <c>CAnnouncerPack</c> element in the game data.</para>
    /// </summary>
    public string AnnouncerPackAttributeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the player's voice line attribute id.
    /// <para>This is the <c>AttributeId</c> value of the <c>CVoiceLine</c> element in the game data.</para>
    /// </summary>
    public string VoiceLineAttributeId { get; set; } = string.Empty;
}
