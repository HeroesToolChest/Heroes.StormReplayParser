namespace Heroes.StormReplayParser;

// from the .s2ml files
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "No need")]
internal enum ReplayAttributeEventType
{
    Controller = 500,
    Rules = 1000,
    IsPremadeGame = 1001,

    Teams = 2000, // Dflt, no
    TeamSize = 2001,
    Team1v1 = 2002,
    Team2v2 = 2003,
    Team3v3 = 2004,
    Team4v4 = 2005,
    TeamFFA = 2006,
    Team5v5 = 2007,
    Team6v6 = 2008,

    TeamT2 = 2011,
    TeamT3 = 2012,
    TeamT4 = 2013,
    TeamT5 = 2014,
    TeamT6 = 2015,
    TeamT7 = 2016,
    TeamFFA2 = 2017, // similar to TeamFFA
    TeamCust = 2018,
    TeamT8 = 2019,
    TeamT9 = 2020,
    TeamFFAT = 2023,
    TeamCuTA = 2024,

    GameSpeed = 3000,
    PlayerRace = 3001,
    Color = 3002,
    PlayerHandicap = 3003,
    DifficultyLevel = 3004,
    ComputerRace = 3005,
    LobbyDelay = 3006,
    ParticipantRole = 3007,
    ObserverType = 3008,
    GameMode = 3009,
    LockedAlliances = 3010,
    PlayerLogoIndex = 3011,
    TandemLeaderSlot = 3012,
    Commander = 3013,
    CommanderLevel = 3014,
    GameDuration = 3015,

    /* 3100 - 3300 are related to AI builds (for Starcraft 2) */

    GamePrivacy = 4000,
    UsingCustomObserverUI = 4001,
    HeroAttributeId = 4002,
    Skin = 4003, // heroes
    Mount = 4004,
    Ready = 4005,
    HeroRange = 4006,
    HeroRole = 4007,
    HeroLevel = 4008,
    CanReady = 4009,
    LobbyMode = 4010,
    ReadyOrder = 4011,
    ReadyingTeam = 4012,
    HeroDuplicatesAllowed = 4013,
    HeroVisibility = 4014,
    LobbyPhase = 4015,
    ReadyingCount = 4016,
    ActiveRound = 4017,
    ReadyMode = 4018,
    ReadyRequirements = 4019,
    FirstDraftTeam = 4020,

    BanMode = 4021,

    Team1BanChooser = 4022,
    Team1Ban1HeroChoice = 4023,
    Team1Ban1LockedIn = 4024,
    Team1Ban2HeroChoice = 4025,
    Team1Ban2LockedIn = 4026,
    Team2BanChooser = 4027,
    Team2Ban1HeroChoice = 4028,
    Team2Ban1LockedIn = 4029,
    Team2Ba2HeroChoice = 4030,
    Team2Ban2LockedIn = 4031,

    Banner = 4032,
    Spray = 4033,
    VoiceLine = 4034,
    AnnouncerPack = 4035,

    TrainingReadyRequirements = 4036,
    VsAIReadyRequirements = 4037,
    QuickMatchReadyRequirements = 4038,
    UnrankedDraftReadyRequirements = 4039,
    HeroLeaugeReadyRequirements = 4040,
    TeamLeaugeReadyRequirements = 4041,
    BrawlReadyRequirements = 4042,

    Team1Ban3HeroChoice = 4043,
    Team1Ban3LockedIn = 4044,
    Team2Ban3HeroChoice = 4045,
    Team2Ban3LockedIn = 4046,

    TankRoleFill = 4047,
    HealerRoleFill = 4048,
    RangedDamageRoleFill = 4049,
    BruiserRoleFill = 4050,
    SupportRoleFill = 4051,
    MeleeDamageRoleFill = 4052,

    ExpandedHeroRole = 4053,
    HeroMasteryRingTier = 4054,
}
