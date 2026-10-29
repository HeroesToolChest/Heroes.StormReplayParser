# Heroes Storm Replay Parser
[![Build](https://github.com/HeroesToolChest/Heroes.StormReplayParser/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/HeroesToolChest/Heroes.StormReplayParser/actions/workflows/build.yml)
[![Release](https://img.shields.io/github/release/HeroesToolChest/Heroes.StormReplayParser.svg)](https://github.com/HeroesToolChest/Heroes.StormReplayParser/releases/latest) 
[![NuGet](https://img.shields.io/nuget/v/Heroes.StormReplayParser.svg)](https://www.nuget.org/packages/Heroes.StormReplayParser/)

Heroes Storm Replay Parser is a .NET library that parses the Heroes of the Storm replay files (`.StormReplay`) and can also be used to parse `.battlelobby` files.

For a CLI tool check out [Heroes Decode](https://github.com/HeroesToolChest/HeroesDecode).

This library was based on [Heroes.ReplayParser](https://github.com/barrett777/Heroes.ReplayParser).

## Replay File Usage
A replay is parsed with `StormReplay.Parse`, which takes either the path to a `.StormReplay` file or a `Stream` of one. It returns a `StormReplayResult` that has the status of the parsing (`Status`), the `StormReplay` with all of the parsed data (`Replay`), and the exception if one occurred (`Exception`).

Errors from opening or parsing the replay are not thrown. Instead, the status is `StormReplayParseStatus.Exception` and `Exception` has the error. The only exceptions thrown are for a path that is `null`, empty, or only white space, or a `null` stream.

### Parsing a File
Example:
```C#
// parse the replay file
StormReplayResult stormReplayResult = StormReplay.Parse(@"C:\<USER PATH>\Replays\Multiplayer\2020-06-29 20.08.13 Garden of Terror.StormReplay");

// get the result of the parsing
StormReplayParseStatus status = stormReplayResult.Status;

// check if it succeeded
if (status == StormReplayParseStatus.Success)
{
    // get the replay object
    StormReplay replay = stormReplayResult.Replay;

    // version of the replay
    StormReplayVersion version = replay.ReplayVersion;

    // map, game mode, and length of the game
    string mapName = replay.MapInfo.MapName;
    StormGameMode gameMode = replay.GameMode;
    TimeSpan length = replay.ReplayLength;

    // player data
    foreach (StormPlayer player in replay.StormPlayers)
    {
        string name = player.Name;
        string? heroName = player.PlayerHero?.HeroName;
        bool isWinner = player.IsWinner;
    }
}
else
{
    // check if the status is an exception
    if (status == StormReplayParseStatus.Exception)
    {
        // the exception
        StormParseException? stormParseException = stormReplayResult.Exception;
    }
}
```

### Parsing a Stream
A replay can also be parsed from a `Stream`, such as a `FileStream`.
- The replay is read from the stream's current position, so it does not have to be at the start of the stream.
- The stream must support seeking.
- The stream is disposed after parsing.

Example:
```C#
// open the replay file as a stream
FileStream stream = File.OpenRead(@"C:\<USER PATH>\Replays\Multiplayer\2020-06-29 20.08.13 Garden of Terror.StormReplay");

// parse the replay from the stream, the stream is disposed after parsing
StormReplayResult stormReplayResult = StormReplay.Parse(stream);

if (stormReplayResult.Status == StormReplayParseStatus.Success)
{
    StormReplay replay = stormReplayResult.Replay;
}
```

### Parsing Options
Besides the replay, a `ParseOptions` may also be passed in. It defines how much of the replay to parse. By default, the tracker, game, and message events are parsed, PTR replays are not allowed, and the license ids are not parsed.

| Option | Default | Description |
| --- | --- | --- |
| `AllowPTR` | `false` | Whether PTR replays are allowed. If `false`, a PTR replay is still parsed, but its status is `PTRRegion` instead of `Success`. |
| `ShouldParseTrackerEvents` | `true` | Whether the tracker events are parsed. |
| `ShouldParseTrackerUnitEvents` | `true` | Whether the unit tracker events (born, died, positions, etc...) are parsed. None of the other replay data comes from them, so setting this to `false` only leaves them out of `TrackerEvents`. |
| `ShouldParseGameEvents` | `true` | Whether the game events are parsed. |
| `ShouldKeepGameEvents` | `true` | Whether the game events are kept in `GameEvents`. If `false`, the game events are still parsed for their data, but are not kept. |
| `ShouldParseMessageEvents` | `true` | Whether the message events are parsed. |
| `ShouldParseLicenseCatalog` | `false` | Whether the license id catalog and the players' license ids are parsed. See [License Ids](#license-ids). |

There are also two preset options:
- `ParseOptions.DefaultParsing` is the same as a new `ParseOptions`.
- `ParseOptions.MinimalParsing` parses the replay the fastest. None of the tracker, game, or message events or the license ids are parsed, but the replay still has its basic data, such as the players and their heroes, the map, the game mode, and the length of the game.

The game events take most of the time and memory of a parse. If the data from the game events is needed, but not the game events themselves, setting `ShouldKeepGameEvents` to `false` makes parsing much faster.

Example:
```C#
ParseOptions parseOptions = new()
{
    ShouldParseTrackerUnitEvents = false,
    ShouldKeepGameEvents = false,
};

StormReplayResult stormReplayResult = StormReplay.Parse(@"C:\<USER PATH>\Replays\Multiplayer\2020-06-29 20.08.13 Garden of Terror.StormReplay", parseOptions);
```

Tracker event parsing provides the following:
- Draft picks (`StormReplay.DraftPicks`)
- Team levels (`StormReplay.GetTeamLevels()`, `StormReplay.GetTeamFinalLevel()`)
- Team xp breakdown (`StormReplay.GetTeamXPBreakdown()`)
- Map id (`StormMapInfo.MapId`)
- Hero unit ids (`PlayerHero.HeroUnitId`)
- Talent ids (`HeroTalent.TalentNameId`)
- Player end of game score results (`StormPlayer.ScoreResult`)
- Player end of game match awards (`StormPlayer.MatchAwards`)
- All of the tracker events (`StormReplay.TrackerEvents`)

Game event parsing provides the following:
- Talent slot ids and timestamps (`HeroTalent.TalentSlotId`, `HeroTalent.Timestamp`)
- Player disconnects (`StormPlayer.PlayerDisconnects`)
- Whether players are on a Mac (`StormPlayer.IsPlatformMac`)
- Team chat messages of both teams (`StormReplay.TeamChatMessages`)
- All of the game events (`StormReplay.GameEvents`), if `ShouldKeepGameEvents` is `true`

Message event parsing provides the following:
- All message types, such as chat, ping, loading progress, and player announce messages (`StormReplay.Messages`)
- Only the chat messages (`StormReplay.ChatMessages`)

### License Ids
Each player has a list of license ids (`StormPlayer.LicenseIds`), which are the collection items (heroes, skins, mounts, etc...) the player has a license for, including the free-to-play heroes. The license id of the player's selected portrait is also available (`StormPlayer.PortraitLicenseId`). The license ids are the `License` values of the `CReward` type entries in the game data.

The license ids require `ShouldParseLicenseCatalog` to be `true`, or a license id catalog to be passed in. The license id catalog (`StormReplay.LicenseIdCatalog`) is the set of all the license ids of a build, so it is the same for all replays of the same build.

A catalog from a previous replay can be passed in to share it with the replays of the same build. If it is from a different build, a new catalog is created from the replay.

Example:
```C#
IReadOnlySet<long>? licenseIdCatalog = null;

foreach (string replayFile in Directory.EnumerateFiles(@"C:\<USER PATH>\Replays\Multiplayer", "*.StormReplay"))
{
    // the catalog of the previous replay is passed in, and is shared if the replay is from the same build
    StormReplayResult stormReplayResult = StormReplay.Parse(replayFile, new ParseOptions() { ShouldParseLicenseCatalog = true }, licenseIdCatalog);

    if (stormReplayResult.Status == StormReplayParseStatus.Success)
    {
        licenseIdCatalog = stormReplayResult.Replay.LicenseIdCatalog;

        foreach (StormPlayer player in stormReplayResult.Replay.StormPlayers)
        {
            IReadOnlyList<long> licenseIds = player.LicenseIds;
        }
    }
}
```

## BattleLobby File Usage
`replay.server.battlelobby` files can also be parsed to obtain data for pregame analysis tools. The `.battlelobby` file is created at the start of the loading screen.

On Windows, the default location is `C:\<USER PATH>\AppData\Local\Temp\Heroes of the Storm\TempWriteReplayP1\replay.server.battlelobby`. The `Temp\Heroes of the Storm` directory is deleted after the game (not the match) has closed.

A battlelobby file is parsed with `StormReplayPregame.Parse`, which takes either the path to a `.battlelobby` file or a `Stream` of one. It returns a `StormReplayPregameResult` that has the status of the parsing (`Status`), the `StormReplayPregame` with all of the parsed data (`ReplayBattleLobby`), and the exception if one occurred (`Exception`).

As with replays, errors are not thrown. Instead, the status is `StormReplayPregameParseStatus.Exception`, which includes a file that is not a battlelobby file.

### Parsing a File
Example:
```C#
// parse the battlelobby file
StormReplayPregameResult stormReplayPregameResult = StormReplayPregame.Parse(@"C:\<USER PATH>\AppData\Local\Temp\Heroes of the Storm\TempWriteReplayP1\replay.server.battlelobby");

// get the result of the parsing
StormReplayPregameParseStatus status = stormReplayPregameResult.Status;

// check if it succeeded
if (status == StormReplayPregameParseStatus.Success)
{
    // get the replay object
    StormReplayPregame replay = stormReplayPregameResult.ReplayBattleLobby;

    // version of the replay
    int buildVersion = replay.ReplayBuild;

    // map (not available for every battlelobby file) and game mode
    string? mapTitle = replay.MapTitle;
    StormGameMode gameMode = replay.GameMode;

    // player data
    IEnumerable<PregameStormPlayer> players = replay.StormPlayers;
}
else
{
    // check if the status is an exception
    if (status == StormReplayPregameParseStatus.Exception)
    {
        // the exception
        StormParseException? stormParseException = stormReplayPregameResult.Exception;
    }
}
```

### Parsing a Stream
A battlelobby file can also be parsed from a `Stream`, such as a `FileStream`.
- The data is read from the stream's current position to its end.
- The stream must support seeking.
- The stream is disposed after parsing.

Example:
```C#
// open the battlelobby file as a stream
FileStream stream = File.OpenRead(@"C:\<USER PATH>\AppData\Local\Temp\Heroes of the Storm\TempWriteReplayP1\replay.server.battlelobby");

// parse the battlelobby file from the stream, the stream is disposed after parsing
StormReplayPregameResult stormReplayPregameResult = StormReplayPregame.Parse(stream);

if (stormReplayPregameResult.Status == StormReplayPregameParseStatus.Success)
{
    StormReplayPregame replay = stormReplayPregameResult.ReplayBattleLobby;
}
```

### Parsing Options
Besides the battlelobby file, a `ParsePregameOptions` may also be passed in.

| Option | Default | Description |
| --- | --- | --- |
| `AllowPTR` | `true` | Whether PTR battlelobby files are allowed. If `false`, a PTR battlelobby file is still parsed, but its status is `PTRRegion` instead of `Success`. |
| `ShouldParseLicenseCatalog` | `false` | Whether the license id catalog and the players' license ids are parsed. A license id catalog can also be passed in, the same as with [replays](#license-ids). |

## Data References
Some data returned will be references to the game data, such as the ids and attribute ids of heroes, talents, and loadout items. Such data can be obtained from [HeroesDataParser](https://github.com/HeroesToolChest/HeroesDataParser). From there other resource links can be found.

Examples, from a build 98348 replay of a player playing Arthas:

| Property | Game data value | Example |
| --- | --- | --- |
| `PlayerHero.HeroId` | `id` of the `CHero` element | `Arthas` |
| `PlayerHero.HeroAttributeId` | `AttributeId` of the `CHero` element | `Arth` |
| `PlayerHero.HeroUnitId` | `id` of the `CUnit` element | `HeroArthas` |
| `HeroTalent.TalentNameId` | `id` of the `CTalent` element | `ArthasMasteryFrostPresenceHowlingBlast` |
| `PlayerLoadout.SkinAndSkinTint` | `id` of the `CSkin` element | `ArthasUltimate` |
| `PlayerLoadout.SkinAndSkinTintAttributeId` | `AttributeId` of the `CSkin` element | `ArtC` |
| `StormPlayer.PortraitLicenseId` | `License` of the `CRewardPortrait` element | `23880` (`DarkRitualPortrait`) |
| `StormPlayer.LicenseIds` | `License` values of the `CReward` type elements | `10109` (`Arthas`) |
| `StormMapInfo.MapId` | the map's id from its stormmod `mapscript.galaxy` file | `BattlefieldOfEternity` |

Each `PlayerLoadout` item (skin, mount, banner, spray, announcer pack, and voice line) has both an id and an attribute id. The `PregamePlayerLoadout` of a battlelobby file only has the attribute ids, such as `PregamePlayerLoadout.MountAndMountTintAttributeId`.

The license ids are numbers, so they are matched to the game data by the `License` value of the `CReward` type elements, such as `CRewardHero`, `CRewardSkin`, and `CRewardPortrait`. The reward license data of HeroesDataParser has the license id of each of these elements. A license id can belong to more than one element.

The portrait and portrait border unlockable ids (`StormPlayer.PortraitUnlockableId`, `StormPlayer.PortraitBorderUnlockableId`) are not from the game data, but from the unlockable files (`.unlk`) of the Battle.net cache.

## Developing
To build and compile the code, it is recommended to use the latest version of [Visual Studio 2026 or Visual Studio Code](https://visualstudio.microsoft.com/downloads/).

Another option is to use the dotnet CLI tools from the latest [.NET SDK](https://dotnet.microsoft.com/download).

## License
[MIT license](/LICENSE)
