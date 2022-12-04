namespace Heroes.StormReplayParser.MessageEvent;

/// <summary>
/// Contains the information for a player announce message.
/// </summary>
public class PlayerAnnounceMessage : StormMessageBase
{
    /// <summary>
    /// Gets the type of announcment.
    /// </summary>
    public AnnouncementType AnnouncementType { get; internal set; }

    /// <summary>
    /// Gets the ability announcement.
    /// </summary>
    public AbilityAnnouncement? AbilityAnnouncement { get; internal set; } = null;

    /// <summary>
    /// Gets the behavior announcment.
    /// </summary>
    public BehaviorAnnouncement? BehaviorAnnouncement { get; internal set; } = null;

    /// <summary>
    /// Gets the vital announcement.
    /// </summary>
    public VitalAnnouncement? VitalAnnouncement { get; internal set; } = null;

    /// <inheritdoc/>
    public override string Message
    {
        get
        {
            if (MessageSender is null)
                return $"({Timestamp}) ((Unknown)): [announced '{AnnouncementType}']";
            else if (!string.IsNullOrEmpty(MessageSender.PlayerHero?.HeroName))
                return $"({Timestamp}) {MessageSender.Name} ({MessageSender.PlayerHero.HeroName}): [announced '{AnnouncementType}']";
            else
                return $"({Timestamp}) {MessageSender.Name}: [announced '{AnnouncementType}']";
        }
    }
}
