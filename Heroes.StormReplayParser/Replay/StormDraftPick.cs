using Heroes.StormReplayParser.Player;

namespace Heroes.StormReplayParser.Replay
{
    /// <summary>
    /// Contains the properties for the draft pick.
    /// </summary>
    public class StormDraftPick
    {
        private StormTeam _team;

        /// <summary>
        /// Gets or sets the name of the selected hero (internal name - CHeroId).
        /// </summary>
        public string HeroSelected { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the player who performed the <see cref="PickType"/>.
        /// </summary>
        public StormPlayer? Player { get; set; } = null;

        /// <summary>
        /// Gets or sets the the team who performed the <see cref="PickType"/>.
        /// </summary>
        public StormTeam Team
        {
            get
            {
                if (Player == null)
                    return _team;
                else
                    return Player.Team;
            }
            set => _team = value;
        }

        /// <summary>
        /// Gets or sets the type of pick.
        /// </summary>
        public StormDraftPickType PickType { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (Player != null)
            {
                if (PickType == StormDraftPickType.Swapped)
                    return $"Player: {Player.Name} - {PickType} to {HeroSelected}";
                else
                    return $"Player: {Player.Name} - {PickType} {HeroSelected}";
            }
            else
            {
                return $"Team: {Team} - {PickType} {HeroSelected}";
            }
        }
    }
}
