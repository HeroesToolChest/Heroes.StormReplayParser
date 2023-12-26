namespace Heroes.StormReplayParser;

internal static class PregameMapper
{
    public static void TransferTo(this StormReplayPregame pregameReplay, StormReplay replay)
    {
        replay.IsBattleLobbyPlayerInfoParsed = pregameReplay.IsBattleLobbyPlayerInfoParsed;

        for (int i = 0; i < pregameReplay.ClientListByWorkingSetSlotID.Length; i++)
        {
            PregameStormPlayer? stormPregamePlayer = pregameReplay.ClientListByWorkingSetSlotID[i];
            if (stormPregamePlayer is null)
                continue;

            if ((stormPregamePlayer.PlayerType == PlayerType.Observer && stormPregamePlayer.PlayerSlotType == PlayerSlotType.Human) ||
                stormPregamePlayer.PlayerSlotType == PlayerSlotType.Human)
            {
                StormPlayer? stormPlayer = replay.ClientListByWorkingSetSlotID[i];
                stormPlayer ??= replay.ClientListByUserID[i];

                if (stormPlayer is null || (stormPregamePlayer.Name != stormPlayer.Name && stormPregamePlayer.ToonHandle!.Id != stormPlayer.ToonHandle?.Id))
                    throw new StormParseException("Mismatch on data transfer from pregame to replay");

                stormPlayer.PartyValue = stormPregamePlayer.PartyValue;
                stormPlayer.AccountLevel = stormPregamePlayer.AccountLevel;
                stormPlayer.BattleTagName = stormPregamePlayer.BattleTagName;
                stormPlayer.IsBlizzardStaff = stormPregamePlayer.IsBlizzardStaff;
                stormPlayer.IsSilenced = stormPregamePlayer.IsSilenced;
                stormPlayer.IsVoiceSilenced = stormPregamePlayer.IsVoiceSilenced;
                stormPlayer.HasActiveBoost = stormPregamePlayer.HasActiveBoost;

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
            }
        }
    }
}
