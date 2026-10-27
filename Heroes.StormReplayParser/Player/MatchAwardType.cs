namespace Heroes.StormReplayParser.Player;

/// <summary>
/// Specifies the match awards types.
/// </summary>
public enum MatchAwardType
{
    /// <summary>
    /// Specifies the MVP award.
    /// </summary>
    MVP = 1,

    /// <summary>
    /// Specifies the <c>Dominator</c> award.
    /// </summary>
    HighestKillStreak = 2,

    /// <summary>
    /// Specifies the <c>Experienced</c> award.
    /// </summary>
    MostXPContribution = 3,

    /// <summary>
    /// Specifies the <c>Painbringer</c> award.
    /// </summary>
    MostHeroDamageDone = 4,

    /// <summary>
    /// Specifies the <c>Siege Master</c> award.
    /// </summary>
    MostSiegeDamageDone = 5,

    /// <summary>
    /// Specifies the <c>Bulwark</c> award.
    /// </summary>
    MostDamageTaken = 6,

    /// <summary>
    /// Specifies the <c>Main Healer</c> award.
    /// </summary>
    MostHealing = 7,

    /// <summary>
    /// Specifies the <c>Stunner</c> award.
    /// </summary>
    MostStuns = 8,

    /// <summary>
    /// Specifies the <c>Headhunter</c> award.
    /// </summary>
    MostMercCampsCaptured = 9,

    // MapSpecific = 10, - Instead of tracking this generic one, just check if the player has one of the other map-specific Match Awards above 1000

    /// <summary>
    /// Specifies the <c>Finisher</c> award.
    /// </summary>
    MostKills = 11,

    /// <summary>
    /// Specifies the <c>Hat Trick</c> award.
    /// </summary>
    HatTrick = 12,

    /// <summary>
    /// Specifies the <c>Clutch Healer</c> award.
    /// </summary>
    ClutchHealer = 13,

    /// <summary>
    /// Specifies the <c>Protector</c> award.
    /// </summary>
    MostProtection = 14,

    /// <summary>
    /// Specifies the <c>Sole Survivor</c> award.
    /// </summary>
    ZeroDeaths = 15,

    /// <summary>
    /// Specifies the <c>Trapper</c> award.
    /// </summary>
    MostRoots = 16,

    /// <summary>
    /// Specifies the <c>Team Player</c> award.
    /// </summary>
    ZeroOutnumberedDeaths = 17,

    /// <summary>
    /// Specifies the <c>Daredevil</c> award.
    /// </summary>
    MostDaredevilEscapes = 18,

    /// <summary>
    /// Specifies the <c>Escape Artist</c> award.
    /// </summary>
    MostEscapes = 19,

    /// <summary>
    /// Specifies the <c>Silencer</c> award.
    /// </summary>
    MostSilences = 20,

    /// <summary>
    /// Specifies the <c>Guardian</c> award.
    /// </summary>
    MostTeamfightDamageTaken = 21,

    /// <summary>
    /// Specifies the <c>Combat Medic</c> award.
    /// </summary>
    MostTeamfightHealingDone = 22,

    /// <summary>
    /// Specifies the <c>Scrapper</c> award.
    /// </summary>
    MostTeamfightHeroDamageDone = 23,

    /// <summary>
    /// Specifies the <c>Avenger</c> award.
    /// </summary>
    MostVengeancesPerformed = 24,

    /// <summary>
    /// Specifies the <c>Immortal Slayer</c> award.
    /// </summary>
    MostImmortalDamage = 1001,

    /// <summary>
    /// Specifies the <c>Moneybags</c> award.
    /// </summary>
    MostCoinsPaid = 1002,

    /// <summary>
    /// Specifies the <c>Master of the Curse</c> award.
    /// </summary>
    MostCurseDamageDone = 1003,

    /// <summary>
    /// Specifies the <c>Shriner</c> award.
    /// </summary>
    MostDragonShrinesCaptured = 1004,

    /// <summary>
    /// Specifies the <c>Garden Terror</c> award.
    /// </summary>
    MostDamageToPlants = 1005,

    /// <summary>
    /// Specifies the <c>Skull Collector</c> award.
    /// </summary>
    MostSkullsCollected = 1006,

    /// <summary>
    /// Specifies the <c>Guardian Slayer</c> award.
    /// </summary>
    MostDamageToMinions = 1007,

    /// <summary>
    /// Specifies the <c>Temple Master</c> award.
    /// </summary>
    MostTimeInTemple = 1008,

    /// <summary>
    /// Specifies the <c>Jeweler</c> award.
    /// </summary>
    MostGemsTurnedIn = 1009,

    /// <summary>
    /// Specifies the <c>Cannoneer</c> award.
    /// </summary>
    MostAltarDamage = 1010,

    // Lost Cavern = 1011 - No map award

    /// <summary>
    /// Specifies the <c>Zerg Crusher</c> award.
    /// </summary>
    MostDamageDoneToZerg = 1012,

    /// <summary>
    /// Specifies the <c>Da Bomb</c> award.
    /// </summary>
    MostNukeDamageDone = 1013,

    /// <summary>
    /// Specifies the <c>Pusher</c> award.
    /// </summary>
    MostTimePushing = 1016,

    /// <summary>
    /// Specifies the <c>Point Guard</c> award.
    /// </summary>
    MostTimeOnPoint = 1019,

    /// <summary>
    /// Specifies the <c>Loyal Defender</c> award.
    /// </summary>
    MostInterruptedCageUnlocks = 1022,

    /// <summary>
    /// Specifies the <c>Seed Collector</c> award.
    /// </summary>
    MostSeedsCollected = 1023,
}
