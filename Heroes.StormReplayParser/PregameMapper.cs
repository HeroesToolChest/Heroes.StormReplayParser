namespace Heroes.StormReplayParser;

internal static class PregameMapper
{
    public static void TransferTo(this StormReplayPregame pregameReplay, StormReplay replay)
    {
        replay.LicenseIdCatalog = pregameReplay.LicenseIdCatalog;
        replay.IsBattleLobbyPlayerInfoParsed = pregameReplay.IsBattleLobbyPlayerInfoParsed;

        int userIndex = 0;
        for (int slotIndex = userIndex; slotIndex < pregameReplay.ClientListByWorkingSetSlotID.Length; slotIndex++)
        {
            PregameStormPlayer? stormPregamePlayer = pregameReplay.ClientListByWorkingSetSlotID[slotIndex];
            if (stormPregamePlayer is null)
                continue;

            if ((stormPregamePlayer.PlayerType != PlayerType.Observer || stormPregamePlayer.PlayerSlotType != PlayerSlotType.Human) &&
                stormPregamePlayer.PlayerSlotType != PlayerSlotType.Human)
            {
                continue;
            }

            StormPlayer? stormPlayer = replay.ClientListByWorkingSetSlotID[slotIndex];
            stormPlayer ??= replay.ClientListByUserID[userIndex];

            if (stormPlayer is null || (stormPregamePlayer.Name != stormPlayer.Name && stormPregamePlayer.ToonHandle!.Id != stormPlayer.ToonHandle?.Id))
                throw new StormParseException("Mismatch on data transfer from pregame to replay");

            stormPlayer.PartyValue = stormPregamePlayer.PartyValue;
            stormPlayer.AccountLevel = stormPregamePlayer.AccountLevel;
            stormPlayer.BattleTagName = stormPregamePlayer.BattleTagName;
            stormPlayer.IsBlizzardStaff = stormPregamePlayer.IsBlizzardStaff;
            stormPlayer.IsSilenced = stormPregamePlayer.IsSilenced;
            stormPlayer.IsVoiceSilenced = stormPregamePlayer.IsVoiceSilenced;
            stormPlayer.HasActiveBoost = stormPregamePlayer.HasActiveBoost;
            stormPlayer.PortraitUnlockableId = stormPregamePlayer.PortraitUnlockableId;
            stormPlayer.PortraitBorderUnlockableId = stormPregamePlayer.PortraitBorderUnlockableId;
            stormPlayer.LicenseIdsInternal = stormPregamePlayer.LicenseIdsInternal;
            stormPlayer.RewardTagsCount = stormPregamePlayer.RewardTagsCount;

            if (stormPregamePlayer.IsReplayOwner)
            {
                stormPlayer.IsReplayOwner = true;
                replay.Owner = stormPlayer;
            }

            stormPlayer.ToonHandle ??= new()
            {
                Realm = stormPregamePlayer.ToonHandle!.Realm,
                Id = stormPregamePlayer.ToonHandle.Id,
                ProgramId = stormPregamePlayer.ToonHandle.ProgramId,
                Region = stormPregamePlayer.ToonHandle.Region,
            };

            stormPlayer.ToonHandle.ShortcutId = stormPregamePlayer.ToonHandle!.ShortcutId;

            if (stormPregamePlayer.PlayerType == PlayerType.Observer)
                stormPlayer.Team = StormTeam.Observer;

            userIndex++;
        }
    }
}
