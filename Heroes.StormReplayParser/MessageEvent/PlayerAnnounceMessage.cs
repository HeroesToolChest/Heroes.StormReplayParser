namespace Heroes.StormReplayParser.MessageEvent
{
    /// <summary>
    /// Contains the information for a player announce message.
    /// </summary>
    public class PlayerAnnounceMessage
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
    }
}
