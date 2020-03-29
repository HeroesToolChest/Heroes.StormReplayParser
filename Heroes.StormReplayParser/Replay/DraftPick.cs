namespace Heroes.StormReplayParser.Replay
{
    /// <summary>
    /// Contains the properties for the draft pick.
    /// </summary>
    public class DraftPick
    {
        /// <summary>
        /// Gets or sets the player slot id of the player who is performing the selection.
        /// </summary>
        public int SelectedPlayerSlotId { get; set; }

        /// <summary>
        /// Gets or sets the name of the selected hero (internal name - CHeroId).
        /// </summary>
        public string HeroSelected { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of pick.
        /// </summary>
        public DraftPickType PickType { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Player: {SelectedPlayerSlotId} - {PickType} {HeroSelected}";
        }
    }
}
