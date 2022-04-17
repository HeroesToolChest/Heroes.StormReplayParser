namespace Heroes.StormReplayParser;

[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "No need")]
internal enum ReplayAttributeEventType
{
    PlayerTypeAttribute = 500,
    Rules = 1000,
    IsPremadeGame = 1001,

    /* 2000 - 2024 are related to team sizes */

    TeamSizeAttribute = 2001,
    PlayerTeam1v1Attribute = 2002,
    PlayerTeam2v2Attribute = 2003,
    PlayerTeam3v3Attribute = 2004,
    PlayerTeam4v4Attribute = 2005,
    PlayerTeamFFAAttribute = 2006,

    GameSpeedAttribute = 3000,
    PlayerRaceAttribute = 3001,
    TeamColorIndexAttribute = 3002,
    PlayerHandicapAttribute = 3003,
    DifficultyLevelAttribute = 3004,
    ComputerRace = 3005,
    LobbyDelay = 3006,
    ParticipantRole = 3007,
    WatcherType = 3008,
    GameModeAttribute = 3009,
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
    SkinAndSkinTintAttributeId = 4003,
    MountAndMountTintAttributeId = 4004,
    Ready = 4005,
    HeroType = 4006,
    HeroRole = 4007,
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

    BannerAttributeId = 4032,
    SprayAttributeId = 4033,
    VoiceLineAttributeId = 4034,
    AnnouncerAttributeId = 4035,

    /* 4036 - 4042 ??? */

    DraftTeam1Ban3 = 4043,
    DraftTeam1Ban3LockedIn = 4044,

    DraftTeam2BanChooserSlot = 4027,
    DraftTeam2Ban1 = 4028,
    DraftTeam2Ban1LockedIn = 4029,
    DraftTeam2Ban2 = 4030,
    DraftTeam2Ban2LockedIn = 4031,
    DraftTeam2Ban3 = 4045,
    DraftTeam2Ban3LockedIn = 4046,

    /* 4047 - 4053 ??? */

    /* 4100 - 4200 are related to Artifacts, no longer in the game */
}
