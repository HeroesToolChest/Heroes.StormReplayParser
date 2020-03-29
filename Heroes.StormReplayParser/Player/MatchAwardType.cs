namespace Heroes.StormReplayParser.Player
{
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
        /// Specifies the Dominator award.
        /// </summary>
        HighestKillStreak = 2,

        /// <summary>
        /// Specifies the Experienced award.
        /// </summary>
        MostXPContribution = 3,

        /// <summary>
        /// Specifies the Painbringer award.
        /// </summary>
        MostHeroDamageDone = 4,

        /// <summary>
        /// Specifies the Siege Master award.
        /// </summary>
        MostSiegeDamageDone = 5,

        /// <summary>
        /// Specifies the Bulwark award.
        /// </summary>
        MostDamageTaken = 6,

        /// <summary>
        /// Specifies the Main Healer award.
        /// </summary>
        MostHealing = 7,

        /// <summary>
        /// Specifies the Stunner award.
        /// </summary>
        MostStuns = 8,

        /// <summary>
        /// Specifies the Headhunter award.
        /// </summary>
        MostMercCampsCaptured = 9,

        // MapSpecific = 10, - Instead of tracking this generic one, just check if the player has one of the other map-specific Match Awards above 1000

        /// <summary>
        /// Specifies the Finisher award.
        /// </summary>
        MostKills = 11,

        /// <summary>
        /// Specifies the Hat Trick award.
        /// </summary>
        HatTrick = 12,

        /// <summary>
        /// Specifies the Clutch Healer award.
        /// </summary>
        ClutchHealer = 13,

        /// <summary>
        /// Specifies the Protector award.
        /// </summary>
        MostProtection = 14,

        /// <summary>
        /// Specifies the Sole Survivor award.
        /// </summary>
        ZeroDeaths = 15,

        /// <summary>
        /// Specifies the Trapper award.
        /// </summary>
        MostRoots = 16,

        /// <summary>
        /// Specifies the Team Player award.
        /// </summary>
        ZeroOutnumberedDeaths = 17,

        /// <summary>
        /// Specifies the Daredevil award.
        /// </summary>
        MostDaredevilEscapes = 18,

        /// <summary>
        /// Specifies the Escape Artist award.
        /// </summary>
        MostEscapes = 19,

        /// <summary>
        /// Specifies the Silencer award.
        /// </summary>
        MostSilences = 20,

        /// <summary>
        /// Specifies the Guardian award.
        /// </summary>
        MostTeamfightDamageTaken = 21,

        /// <summary>
        /// Specifies the Combat Medic award.
        /// </summary>
        MostTeamfightHealingDone = 22,

        /// <summary>
        /// Specifies the Scrapper award.
        /// </summary>
        MostTeamfightHeroDamageDone = 23,

        /// <summary>
        /// Specifies the Avenger award.
        /// </summary>
        MostVengeancesPerformed = 24,

        /// <summary>
        /// Specifies the Immortal Slayer award.
        /// </summary>
        MostImmortalDamage = 1001,

        /// <summary>
        /// Specifies the Moneybags award.
        /// </summary>
        MostCoinsPaid = 1002,

        /// <summary>
        /// Specifies the Master of the Curse award.
        /// </summary>
        MostCurseDamageDone = 1003,

        /// <summary>
        /// Specifies the Shriner award.
        /// </summary>
        MostDragonShrinesCaptured = 1004,

        /// <summary>
        /// Specifies the Garden Terror award.
        /// </summary>
        MostDamageToPlants = 1005,

        /// <summary>
        /// Specifies the Skull Collector award.
        /// </summary>
        MostSkullsCollected = 1006,

        /// <summary>
        /// Specifies the Guardian Slayer award.
        /// </summary>
        MostDamageToMinions = 1007,

        /// <summary>
        /// Specifies the Temple Master award.
        /// </summary>
        MostTimeInTemple = 1008,

        /// <summary>
        /// Specifies the Jeweler award.
        /// </summary>
        MostGemsTurnedIn = 1009,

        /// <summary>
        /// Specifies the Cannoneer award.
        /// </summary>
        MostAltarDamage = 1010,

        // Lost Cavern = 1011 - No map award

        /// <summary>
        /// Specifies the Zerg Crusher award.
        /// </summary>
        MostDamageDoneToZerg = 1012,

        /// <summary>
        /// Specifies the Da Bomb award.
        /// </summary>
        MostNukeDamageDone = 1013,

        /// <summary>
        /// Specifies the Pusher award.
        /// </summary>
        MostTimePushing = 1016,

        /// <summary>
        /// Specifies the Point Guard award.
        /// </summary>
        MostTimeOnPoint = 1019,

        /// <summary>
        /// Specifies the Loyal Defender award.
        /// </summary>
        MostInterruptedCageUnlocks = 1022,

        /// <summary>
        /// Specifies the Seed Collector award.
        /// </summary>
        MostSeedsCollected = 1023,
    }
}
