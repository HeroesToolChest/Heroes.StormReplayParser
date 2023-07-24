namespace Heroes.StormReplayParser;

internal static class PregameMapper
{
    public static void TransferTo(this StormReplayPregame pregameReplay, StormReplay replay)
    {
        replay.MapInfo.MapLink = pregameReplay.MapLink;

        for (int i = 0; i < pregameReplay.ClientListByUserID.Length; i++)
        {
            StormPregamePlayer stormPregamePlayer = pregameReplay.ClientListByUserID[i];

            if (stormPregamePlayer.PlayerType == PlayerType.Closed)
                continue;

            StormPlayer stormPlayer = replay.ClientListByUserID[i];

            stormPlayer.PartyValue = stormPregamePlayer.PartyValue;
            stormPlayer.AccountLevel = stormPregamePlayer.AccountLevel;
            stormPlayer.BattleTagName = stormPregamePlayer.BattleTagName;
        }
    }
}
