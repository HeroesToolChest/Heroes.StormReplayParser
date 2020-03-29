namespace Heroes.StormReplayParser.GameEvent
{
    /// <summary>
    /// Specifies the game event type.
    /// </summary>
    public enum StormGameEventType
    {
        /// <summary>
        /// Indicates a user finished loading sync event.
        /// </summary>
        SUserFinishedLoadingSyncEvent = 5,

        /// <summary>
        /// Indicates a user options event.
        /// </summary>
        SUserOptionsEvent = 7,

        /// <summary>
        /// Indicates a bank file event.
        /// </summary>
        SBankFileEvent = 9,

        /// <summary>
        /// Indicates a bank section event.
        /// </summary>
        SBankSectionEvent = 10,

        /// <summary>
        /// Indicates a bank key event.
        /// </summary>
        SBankKeyEvent = 11,

        /// <summary>
        /// Indicates a bank value event.
        /// </summary>
        SBankValueEvent = 12,

        /// <summary>
        /// Indicates a bank signature event.
        /// </summary>
        SBankSignatureEvent = 13,

        /// <summary>
        /// Indicates a camera save event.
        /// </summary>
        SCameraSaveEvent = 14,

        /// <summary>
        /// Indicates a save game event.
        /// </summary>
        SSaveGameEvent = 21,

        /// <summary>
        /// Indicates a save game done event.
        /// </summary>
        SSaveGameDoneEvent = 22,

        /// <summary>
        /// Indicates a load game done event.
        /// </summary>
        SLoadGameDoneEvent = 23,

        /// <summary>
        /// Indicates a command manager reset event.
        /// </summary>
        SCommandManagerResetEvent = 25,

        /// <summary>
        /// Indicates a game cheat event.
        /// </summary>
        SGameCheatEvent = 26,

        /// <summary>
        /// Indicates a command event event.
        /// </summary>
        SCmdEvent = 27,

        /// <summary>
        /// Indicates a selection delta event.
        /// </summary>
        SSelectionDeltaEvent = 28,

        /// <summary>
        /// Indicates a control group update event.
        /// </summary>
        SControlGroupUpdateEvent = 29,

        /// <summary>
        /// Indicates a selection sync check event.
        /// </summary>
        SSelectionSyncCheckEvent = 30,

        /// <summary>
        /// Indicates a trigger chat message event.
        /// </summary>
        STriggerChatMessageEvent = 32,

        /// <summary>
        /// Indicates a dynamic button swap event.
        /// </summary>
        SDynamicButtonSwapEvent = 33,

        /// <summary>
        /// Indicates a set absolute game speed event.
        /// </summary>
        SSetAbsoluteGameSpeedEvent = 34,

        /// <summary>
        /// Indicates an add absolute game speed event.
        /// </summary>
        SAddAbsoluteGameSpeedEvent = 35,

        /// <summary>
        /// Indicates a trigger ping event.
        /// </summary>
        STriggerPingEvent = 36,

        /// <summary>
        /// Indicates a broadcast cheat event.
        /// </summary>
        SBroadcastCheatEvent = 37,

        /// <summary>
        /// Indicates an alliance event.
        /// </summary>
        SAllianceEvent = 38,

        /// <summary>
        /// Indicates a unit click event.
        /// </summary>
        SUnitClickEvent = 39,

        /// <summary>
        /// Indicates a unit highlight event.
        /// </summary>
        SUnitHighlightEvent = 40,

        /// <summary>
        /// Indicates a trigger reply selected event.
        /// </summary>
        STriggerReplySelectedEvent = 41,

        /// <summary>
        /// Indicates a hijack replay game event.
        /// </summary>
        SHijackReplayGameEvent = 43,

        /// <summary>
        /// Indicates a trigger skipped event.
        /// </summary>
        STriggerSkippedEvent = 44,

        /// <summary>
        /// Indicates a trigger sound length query event.
        /// </summary>
        STriggerSoundLengthQueryEvent = 45,

        /// <summary>
        /// Indicates a trigger sound offset event.
        /// </summary>
        STriggerSoundOffsetEvent = 46,

        /// <summary>
        /// Indicates a trigger transmission offset event.
        /// </summary>
        STriggerTransmissionOffsetEvent = 47,

        /// <summary>
        /// Indicates a trigger transmission completed event.
        /// </summary>
        STriggerTransmissionCompleteEvent = 48,

        /// <summary>
        /// Indicates a camera update event.
        /// </summary>
        SCameraUpdateEvent = 49,

        /// <summary>
        /// Indicates a trigger abort mission event.
        /// </summary>
        STriggerAbortMissionEvent = 50,

        /// <summary>
        /// Indicates a trigger dialog control event.
        /// </summary>
        STriggerDialogControlEvent = 55,

        /// <summary>
        /// Indicates a trigger sound length sync event.
        /// </summary>
        STriggerSoundLengthSyncEvent = 56,

        /// <summary>
        /// Indicates a trigger conversation skipped event.
        /// </summary>
        STriggerConversationSkippedEvent = 57,

        /// <summary>
        /// Indicates a trigger mouse clicked event.
        /// </summary>
        STriggerMouseClickedEvent = 58,

        /// <summary>
        /// Indicates a trigger mouse moved event.
        /// </summary>
        STriggerMouseMovedEvent = 59,

        /// <summary>
        /// Indicates an achievement awarded event.
        /// </summary>
        SAchievementAwardedEvent = 60,

        /// <summary>
        /// Indicates a trigger hotkey pressed event.
        /// </summary>
        STriggerHotkeyPressedEvent = 61,

        /// <summary>
        /// Indicates a trigger target mode updated event.
        /// </summary>
        STriggerTargetModeUpdateEvent = 62,

        /// <summary>
        /// Indicates a trigger soundtrack done event.
        /// </summary>
        STriggerSoundtrackDoneEvent = 64,

        /// <summary>
        /// Indicates a trigger key pressed event.
        /// </summary>
        STriggerKeyPressedEvent = 66,

        /// <summary>
        /// Indicates a trigger movie function event.
        /// </summary>
        STriggerMovieFunctionEvent = 67,

        /// <summary>
        /// Indicates a trigger command errors event.
        /// </summary>
        STriggerCommandErrorEvent = 76,

        /// <summary>
        /// Indicates a trigger movie started event.
        /// </summary>
        STriggerMovieStartedEvent = 86,

        /// <summary>
        /// Indicates a triggerm movie finished event.
        /// </summary>
        STriggerMovieFinishedEvent = 87,

        /// <summary>
        /// Indicates a decrement game time remaining event.
        /// </summary>
        SDecrementGameTimeRemainingEvent = 88,

        /// <summary>
        /// Indicates a trigger portrait loaded event.
        /// </summary>
        STriggerPortraitLoadedEvent = 89,

        /// <summary>
        /// Indicates a trigger custom dialog dismissed event.
        /// </summary>
        STriggerCustomDialogDismissedEvent = 90,

        /// <summary>
        /// Indicates a trigger game menu item selected event.
        /// </summary>
        STriggerGameMenuItemSelectedEvent = 91,

        /// <summary>
        /// Indicates a trigger mouse wheel event.
        /// </summary>
        STriggerMouseWheelEvent = 92,

        /// <summary>
        /// Indicates a trigger button pressed event.
        /// </summary>
        STriggerButtonPressedEvent = 95,

        /// <summary>
        /// Indicates a trigger game credits finished event.
        /// </summary>
        STriggerGameCreditsFinishedEvent = 96,

        /// <summary>
        /// Indicates a trigger cut scene bookmark fired event.
        /// </summary>
        STriggerCutsceneBookmarkFiredEvent = 97,

        /// <summary>
        /// Indicates a trigger cutscene ended event.
        /// </summary>
        STriggerCutsceneEndSceneFiredEvent = 98,

        /// <summary>
        /// Indicates a trigger cutscene conversation line event.
        /// </summary>
        STriggerCutsceneConversationLineEvent = 99,

        /// <summary>
        /// Indicates a trigger cutscene conversation line missing event.
        /// </summary>
        STriggerCutsceneConversationLineMissingEvent = 100,

        /// <summary>
        /// Indicates a user leave event.
        /// </summary>
        SGameUserLeaveEvent = 101,

        /// <summary>
        /// Indicates a user joined event.
        /// </summary>
        SGameUserJoinEvent = 102,

        /// <summary>
        /// Indicates a command manager state event.
        /// </summary>
        SCommandManagerStateEvent = 103,

        /// <summary>
        /// Indicates a command update target point event.
        /// </summary>
        SCmdUpdateTargetPointEvent = 104,

        /// <summary>
        /// Indicates a command update target unit event.
        /// </summary>
        SCmdUpdateTargetUnitEvent = 105,

        /// <summary>
        /// Indicates an animation length query by name event.
        /// </summary>
        STriggerAnimLengthQueryByNameEvent = 106,

        /// <summary>
        /// Indicates an animation length query by props event.
        /// </summary>
        STriggerAnimLengthQueryByPropsEvent = 107,

        /// <summary>
        /// Indicates a trigger animation offset event.
        /// </summary>
        STriggerAnimOffsetEvent = 108,

        /// <summary>
        /// Indicates a catalog modify event.
        /// </summary>
        SCatalogModifyEvent = 109,

        /// <summary>
        /// Indicates a hero talent tree selected event.
        /// </summary>
        SHeroTalentTreeSelectedEvent = 110,

        /// <summary>
        /// Indicates a trigger profiler logging finished event.
        /// </summary>
        STriggerProfilerLoggingFinishedEvent = 111,

        /// <summary>
        /// Indicates a hero talent tree selection panel toggled event.
        /// </summary>
        SHeroTalentTreeSelectionPanelToggledEvent = 112,
    }
}
