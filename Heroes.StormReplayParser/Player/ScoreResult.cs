namespace Heroes.StormReplayParser.Player;

/// <summary>
/// Contains the information for a player's final score result of various end game data.
/// </summary>
public class ScoreResult
{
    /// <summary>
    /// Gets the score results that are validated but not set as a property of the <see cref="ScoreResult"/> class.
    /// </summary>
    public Dictionary<string, int> MiscellaneousScoreResultEvents { get; } = new Dictionary<string, int>();

    /// <summary>
    /// Gets the score results that have yet to be validated.
    /// </summary>
    public Dictionary<string, int> NewScoreResultEvents { get; } = new Dictionary<string, int>();

    /// <summary>
    /// Gets or sets the hero level.
    /// </summary>
    public int Level { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of takedowns.
    /// </summary>
    public int Takedowns { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of solo kills.
    /// </summary>
    public int SoloKills { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of assists.
    /// </summary>
    public int Assists { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of deaths.
    /// </summary>
    public int Deaths { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of damage dealt to heroes.
    /// </summary>
    public int HeroDamage { get; set; } = 0;

    /// <summary>
    /// Gets or sets the total amount of damage dealt to minions, structures, and summons.
    /// </summary>
    public int SiegeDamage { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of damage dealt to structures.
    /// </summary>
    public int StructureDamage { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of damage dealt to minions.
    /// </summary>
    public int MinionDamage { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of damage dealt to defending mercenaries.
    /// </summary>
    public int CreepDamage { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of damage dealt to summon units.
    /// </summary>
    public int SummonDamage { get; set; } = 0;

    /// <summary>
    /// Gets or sets the total amount of time CC'ing heroes.
    /// </summary>
    public TimeSpan TimeCCdEnemyHeroes { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Gets or sets the amount of healing.
    /// </summary>
    public int Healing { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of healing done to self.
    /// </summary>
    public int SelfHealing { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of damage taken.
    /// </summary>
    public int DamageTaken { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of damage soaked.
    /// </summary>
    public int DamageSoaked { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of experience earned.
    /// </summary>
    public int ExperienceContribution { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of forts and keeps destroyed.
    /// </summary>
    public int TownKills { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of time spent while being dead.
    /// </summary>
    public TimeSpan TimeSpentDead { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Gets or sets the amount of times a mercenary camp was captured.
    /// </summary>
    public int MercCampCaptures { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of times a watch tower was captured.
    /// </summary>
    public int WatchTowerCaptures { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount experience that will be added to the players account and level.
    /// This is the total experience for the player's team.
    /// </summary>
    public int MetaExperience { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of physical damage dealt.
    /// </summary>
    public int? PhysicalDamage { get; set; } = null;

    /// <summary>
    /// Gets or sets the amount of spell damage dealt.
    /// </summary>
    public int? SpellDamage { get; set; } = null;

    /// <summary>
    /// Gets or sets the amount of time the player was on fire.
    /// </summary>
    public TimeSpan? OnFireTimeonFire { get; set; } = null;

    /// <summary>
    /// Gets or sets the amount of minion kills.
    /// </summary>
    public int MinionKills { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of regeneration globes collected.
    /// </summary>
    public int RegenGlobes { get; set; } = 0;

    /// <summary>
    /// Gets or sets the tier 1 selected talent sort value.
    /// </summary>
    public int? Tier1Talent { get; set; } = null;

    /// <summary>
    /// Gets or sets the tier 4 selected talent sort value.
    /// </summary>
    public int? Tier4Talent { get; set; } = null;

    /// <summary>
    /// Gets or sets the tier 7 selected talent sort value.
    /// </summary>
    public int? Tier7Talent { get; set; } = null;

    /// <summary>
    /// Gets or sets the tier 10 selected talent sort value.
    /// </summary>
    public int? Tier10Talent { get; set; } = null;

    /// <summary>
    /// Gets or sets the tier 13 selected talent sort value.
    /// </summary>
    public int? Tier13Talent { get; set; } = null;

    /// <summary>
    /// Gets or sets the tier 16 selected talent sort value.
    /// </summary>
    public int? Tier16Talent { get; set; } = null;

    /// <summary>
    /// Gets or sets the tier 20 selected talent sort value.
    /// </summary>
    public int? Tier20Talent { get; set; } = null;

    /// <summary>
    /// Gets or sets the amount of the highest streak of kills.
    /// </summary>
    public int HighestKillStreak { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of healing given to allies.
    /// </summary>
    public int ProtectionGivenToAllies { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of time silencing enemy heroes.
    /// </summary>
    public TimeSpan TimeSilencingEnemyHeroes { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Gets or sets the amount of time rooting enemey heroes.
    /// </summary>
    public TimeSpan TimeRootingEnemyHeroes { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Gets or sets the amount of time stunning enemy heroes.
    /// </summary>
    public TimeSpan TimeStunningEnemyHeroes { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Gets or sets the amount of clutch heals performed.
    /// </summary>
    public int ClutchHealsPerformed { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of times escapes were performed.
    /// </summary>
    public int EscapesPerformed { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of times a vengeance was performed.
    /// </summary>
    public int VengeancesPerformed { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount times of outnumbered deaths.
    /// </summary>
    public int OutnumberedDeaths { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of team fight escapes performed.
    /// </summary>
    public int TeamfightEscapesPerformed { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of team fight healing.
    /// </summary>
    public int TeamfightHealingDone { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of team fight damage taken.
    /// </summary>
    public int TeamfightDamageTaken { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of team fight hero damage.
    /// </summary>
    public int TeamfightHeroDamage { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of multi-kills performs.
    /// </summary>
    public int Multikill { get; set; } = 0;

    internal HashSet<MatchAwardType> MatchAwards { get; set; } = new HashSet<MatchAwardType>();
}
