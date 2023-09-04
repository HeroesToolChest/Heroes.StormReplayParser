# Heroes Storm Replay Parser
[![Build Status](https://dev.azure.com/kevinkoliva/Heroes%20of%20the%20Storm%20Projects/_apis/build/status/HeroesToolChest.Heroes.StormReplayParser?branchName=master)](https://dev.azure.com/kevinkoliva/Heroes%20of%20the%20Storm%20Projects/_build/latest?definitionId=10&branchName=master)
[![Release](https://img.shields.io/github/release/HeroesToolChest/Heroes.StormReplayParser.svg)](https://github.com/HeroesToolChest/Heroes.StormReplayParser/releases/latest) 
[![NuGet](https://img.shields.io/nuget/v/Heroes.StormReplayParser.svg)](https://www.nuget.org/packages/Heroes.StormReplayParser/)

Heroes Storm Replay Parser is a .NET library that parses the Heroes of the Storm replay files (.StormReplay).

To see this library in action check out [Heroes Decode](https://github.com/HeroesToolChest/HeroesDecode).

This library is based on [Heroes.ReplayParser](https://github.com/barrett777/Heroes.ReplayParser).

## Replay File Usage
To parse a replay file use `StormReplay.Parse(string fileName)` by providing the `.StormReplay` file. It will return a `StormReplayResult` object that will have the result of the parsing as well as the `StormReplay` object that will contain all the data parsed.

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

    // player data
    IEnumerable<StormPlayer> players = replay.StormPlayers;
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
### Parsing Options
Besides providing the file name of the replay, `ParseOptions()` may also be passed in. This defines how much of the replay to parse. By default tracker, game, and message events are enabled and PTR parsing is disabled. Parsing a PTR replay will result in a `StormReplayParseStatus` of `PTRRegion`.

Game Event parsing provides the following:
- Hero names (localized)
- Talent timestamps

Tracker Event parsing provides the following:
- Hero unit ids
- Map id name
- Team levels
- Team xp breakdown
- Draft picks
- Talent ids
- Player end of game score results
- Player end of game match awards

Message Events provides the following:
- All message types (chat, ping, player announce messages, etc...)

The above provided are properties of classes that are automatically parsed out. `GameEvents`, `TrackerEvents`, and `Messages` are also properties that are available that can be used for obtaining specific data.

## BattleLobby File Usage
`replay.server.battlelobby` files can also be parsed to obtain data for pregame analysis tools. The `.battlelobby` file is created at the start of the loading screen.

On Windows, the default location is `C:\<USER PATH>\AppData\Local\Temp\Heroes of the Storm\TempWriteReplayP1\replay.server.battlelobby`. The `Temp\Heroes of the Storm` directory is deleted after the game has closed.

To parse a battlelobby file use `StormReplayPregame.Parse(string fileName)` by providing the `.battlelobby` file. It will return a `StormReplayPregameResult` object that will have the result of the parsing as well as the `StormReplayPregame` object that will contain all the data parsed.

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

    // player data
    IEnumerable<StormPregamePlayer> players = replay.StormPlayers;
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

### Parsing Options
Besides providing the file name of the replay, `ParsePregameOptions()` may also be passed in. By default PTR parsing is enabled. Parsing a PTR battlelobby will result in a `StormReplayParseStatus` of `PTRRegion`.

## Data References
Some data returned will be references to the game data, such as `PlayerHero.HeroAttributeId`. Such data can be obtained from [HeroesDataParser](https://github.com/HeroesToolChest/HeroesDataParser). From there other resource links can be found.

## Developing
To build and compile the code, it is recommended to use the latest version of [Visual Studio 2022 or Visual Studio Code](https://visualstudio.microsoft.com/downloads/).

Another option is to use the dotnet CLI tools from the [.NET Core 6.0 SDK](https://dotnet.microsoft.com/download).

## License
[MIT license](/LICENSE)
