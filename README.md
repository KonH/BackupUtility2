# BackupUtility2

## Summary

Utility to backup files using .NET Core

## Features

- Save last N copies of each file in directory beside
- Local and network shared file transfer
- Simple JSON file based configuration
- Skip non-changed files (using __cache.json file in destination location)
- Fast straightforward implementation
- Small codebase
- Low resources usage

## Limitations
- Don't support ignore lists
- Don't delete files when depth limit was decreased, but file not changed

## Usage

```
cd ConsoleRunner
dotnet run
```

## Config example

Config **config.json** should be beside running program, example:

```
{
  "HistoryDepth": 10,
  "Pathes": {
    "...from": "...to"
  }
}
```

- **HistoryDepth** - how many copies will be saved except the last one (it saved into __fileName directory beside, named with changed time suffix)
- **Pathes** - set of pathes, where required to backup data is located and where it will be saved