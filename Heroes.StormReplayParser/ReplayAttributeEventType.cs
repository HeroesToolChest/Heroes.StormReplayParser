namespace Heroes.StormReplayParser;

[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "No need")]
internal enum ReplayAttributeEventType
{
    PlayerType = 500,
    Rules = 1000,
    IsPremadeGame = 1001,

    /* 2000 - 2024 are related to team sizes */

    TeamSize = 2001,
    PlayerTeam1v1 = 2002,
    PlayerTeam2v2 = 2003,
    PlayerTeam3v3 = 2004,
    PlayerTeam4v4 = 2005,
    PlayerTeamFFA = 2006,

    GameSpeed = 3000,
    PlayerRace = 3001,
    TeamColorIndex = 3002,
    PlayerHandicap = 3003,
    DifficultyLevel = 3004,
    ComputerRace = 3005,
    LobbyDelay = 3006,
    ParticipantRole = 3007,
    WatcherType = 3008,
    GameMode = 3009,
    LockedAlliances = 3010,
    PlayerLogo = 3011,
    TandemLeader = 3012,
    Commander = 3013,
    CommanderLevel = 3014,
    GameDuration = 3015,

    /* 3100 - 3300 are related to AI builds (for Starcraft 2) */

    PrivacyOption = 4000,
    UsingCustomObserverUI = 4001,
    HeroAttributeId = 4002,
    SkinAndSkinTint = 4003,
    MountAndMountTint = 4004,
    Ready = 4005,
    HeroType = 4006,
    HeroGeneralRole = 4007,
    HeroLevel = 4008,
    CanReady = 4009,
    LobbyMode = 4010,
    ReadyOrder = 4011,
    ReadyingTeam = 4012,
    HeroDuplicates = 4013,
    HeroVisibility = 4014,
    LobbyPhase = 4015,
    ReadyingCount = 4016,
    ReadyingRound = 4017,
    ReadyMode = 4018,
    ReadyRequirements = 4019,
    FirstReadyingTeam = 4020,

    DraftBanMode = 4021,

    DraftTeam1BanChooserSlot = 4022,
    DraftTeam1Ban1 = 4023,
    DraftTeam1Ban1LockedIn = 4024,
    DraftTeam1Ban2 = 4025,
    DraftTeam1Ban2LockedIn = 4026,

    Banner = 4032,
    Spray = 4033,
    VoiceLine = 4034,
    Announcer = 4035,

    DraftTeam1Ban3 = 4043,
    DraftTeam1Ban3LockedIn = 4044,

    DraftTeam2BanChooserSlot = 4027,
    DraftTeam2Ban1 = 4028,
    DraftTeam2Ban1LockedIn = 4029,
    DraftTeam2Ban2 = 4030,
    DraftTeam2Ban2LockedIn = 4031,
    DraftTeam2Ban3 = 4045,
    DraftTeam2Ban3LockedIn = 4046,

    HeroSpecificRole = 4053,
    HeroMasteryRingTier = 4054,

    /* 4100 - 4200 are related to Artifacts, no longer in the game */
}
