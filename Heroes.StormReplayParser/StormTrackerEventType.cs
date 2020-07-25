namespace Heroes.StormReplayParser
{
    /// <summary>
    /// Specifies the event type.
    /// </summary>
    public enum StormTrackerEventType
    {
        /// <summary>
        /// Indicates a unit born event.
        /// </summary>
        /// <remarks>UnitID Index, UnitID Recycle, Unit Type Name, PlayerID with Control, PlayerID with Upkeep, X, Y</remarks>
        UnitBornEvent = 1,

        /// <summary>
        /// Indicates a unit died event.
        /// </summary>
        /// <remarks>UnitID Index, UnitID Recycle, PlayerID that Killed This, X, Y, Killing UnitID Index, Killing UnitID Recycle</remarks>
        UnitDiedEvent = 2,

        /// <summary>
        /// Indicates a unit owner change event.
        /// </summary>
        /// <remarks>UnitID Index, UnitID Recycle, New PlayerID with Control, New PlayerID with Upkeep</remarks>
        UnitOwnerChangeEvent = 3,

        /// <summary>
        /// Indicates a unit type change event.
        /// </summary>
        /// <remarks>UnitID Index, UnitID Recycle, New Unit Type Name</remarks>
        UnitTypeChangeEvent = 4,

        /// <summary>
        /// Indicates an upgraded event.
        /// </summary>
        /// <remarks>PlayerID, Upgrade Type Name, Count</remarks>
        UpgradeEvent = 5,

        /// <summary>
        /// Indicates a unit init event.
        /// </summary>
        /// <remarks>nitID, Unit Type Name, PlayerID with Control, PlayerID with Upkeep, X, Y</remarks>
        UnitInitEvent = 6,

        /// <summary>
        /// Indicates a unit done event.
        /// </summary>
        /// <remarks>UnitID</remarks>
        UnitDoneEvent = 7,

        /// <summary>
        /// Indicates a unit position event.
        /// </summary>
        /// <remarks>First UnitID Index, Items Array (UnitID Index Offset, X, Y)</remarks>
        UnitPositionsEvent = 8,

        /// <summary>
        /// Indicates a player setup event.
        /// </summary>
        /// <remarks>PlayerID, Player Type (1=Human, 2=CPU, 3=Neutral, 4=Hostile), UserID, SlotID</remarks>
        PlayerSetupEvent = 9,

        /// <summary>
        /// Indicates a state game event.
        /// </summary>
        /// <remarks>EventName, StringData, InitData, FixedData</remarks>
        StatGameEvent = 10,

        /// <summary>
        /// Indicates a score result event.
        /// </summary>
        /// <remarks>InstanceList (20+ length array of Name/Value pairs)</remarks>
        ScoreResultEvent = 11,

        /// <summary>
        /// Indicates a unit revived event.
        /// </summary>
        /// <remarks>UnitID, X, Y</remarks>
        UnitRevivedEvent = 12,

        /// <summary>
        /// Indicates a hero banned event.
        /// </summary>
        /// <remarks>Hero, ControllingTeam</remarks>
        HeroBannedEvent = 13,

        /// <summary>
        /// Indicates a hero picked event.
        /// </summary>
        /// <remarks>Hero, ControllingPlayer</remarks>
        HeroPickedEvent = 14,

        /// <summary>
        /// Indicates a hero swapped event.
        /// </summary>
        /// <remarks>Hero, NewControllingPlayer</remarks>
        HeroSwappedEvent = 15,
    }
}
