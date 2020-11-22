using System;

namespace Heroes.StormReplayParser.Replay
{
    /// <summary>
    /// Specifies the game mode type.
    /// </summary>
    [Flags]
    public enum StormGameMode
    {
        /// <summary>
        /// Indicates an unknown game mode.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Indicates an event game mode.
        /// </summary>
        Event = 1 << 0,

        /// <summary>
        /// Indicates a custom game mode.
        /// </summary>
        Custom = 1 << 1,

        /// <summary>
        /// Indicates a try me game mode.
        /// </summary>
        TryMe = 1 << 2,

        /// <summary>
        /// Indicates a practice game mode.
        /// </summary>
        Practice = 1 << 3,

        /// <summary>
        /// Indicates a cooperative game mode.
        /// </summary>
        Cooperative = 1 << 4,

        /// <summary>
        /// Indicates a quick match game mode.
        /// </summary>
        QuickMatch = 1 << 5,

        /// <summary>
        /// Indicates a hero league game mode.
        /// </summary>
        HeroLeague = 1 << 6,

        /// <summary>
        /// Indicates a team league game mode.
        /// </summary>
        TeamLeague = 1 << 7,

        /// <summary>
        /// Indicates an unranked game mode.
        /// </summary>
        UnrankedDraft = 1 << 8,

        /// <summary>
        /// Indicates a brawl game mode.
        /// </summary>
        Brawl = 1 << 9,

        /// <summary>
        /// Indicates a storm league game mode.
        /// </summary>
        StormLeague = 1 << 10,

        /// <summary>
        /// Indicates an ARAM game mode.
        /// </summary>
        ARAM = 1 << 11,

        /// <summary>
        /// Indicates all of the games modes available.
        /// </summary>
        AllGameModes = Custom | QuickMatch | HeroLeague | TeamLeague | UnrankedDraft | Brawl | StormLeague | Cooperative | ARAM,

        /// <summary>
        /// Indicates game modes that are not brawl, AI, or custom.
        /// </summary>
        NormalGameModes = QuickMatch | HeroLeague | TeamLeague | UnrankedDraft | StormLeague | ARAM,

        /// <summary>
        /// Indicates game modes that involve drafting (includes custom).
        /// </summary>
        DraftModes = HeroLeague | TeamLeague | UnrankedDraft | Custom | StormLeague,

        /// <summary>
        /// Indicates game modes that are ranked.
        /// </summary>
        RankedModes = HeroLeague | TeamLeague | StormLeague,
    }
}
