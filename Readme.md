<img src="Toca3DifficultyEditor/icon.ico" height="24" alt="Program icon" /> ToCA 3 Race Driver Difficulty Editor
===

[![GitHub Actions](https://img.shields.io/github/actions/workflow/status/Aldaviva/Toca3DifficultyEditor/dotnet.yml?branch=master&logo=github)](https://github.com/Aldaviva/Toca3DifficultyEditor/actions/workflows/dotnet.yml)

This program lets you modify the difficulty level of the AI drivers in [ToCA Race Driver 3](https://www.pcgamingwiki.com/wiki/TOCA_Race_Driver_3) by editing game files. The game AI is impossibly difficult on its own, and there is no built-in way to fix it.

## Prerequisites
- [ToCA Race Driver 3](https://www.myabandonware.com/game/toca-race-driver-3-eoh)
    - Tested with version 1.0
    - Not tested with version 1.1 because it was very unstable for me
    - Not tested with DTM Race Driver 3 or V8 Supercars 3 because I don't have those
- [.NET 8.0 Runtime x64 for Windows](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or later

## Installation
1. Download [`Toca3DifficultyEditor.exe`](https://github.com/Aldaviva/Toca3DifficultyEditor/releases/latest/download/Toca3DifficultyEditor.exe) from the [latest release](https://github.com/Aldaviva/Toca3DifficultyEditor/releases/latest).

## Usage
1. Back up `gamedata\frontend\Mods.ini` and `gamedata\chamship\champ.big` from the game installation directory so that they can be reverted later if you want.
1. Exit ToCA Race Driver 3 if it's running.
1. Open a terminal, such as PowerShell or Command Prompt, in the directory where you downloaded `Toca3DifficultyEditor.exe`.
1. Run `Toca3DifficultyEditor.exe` with arguments specifying what difficulty you want.
    - For example, to make the game as easy as possible, you can run
        ```bat
        Toca3DifficultyEditor --career-difficulty 60 --ai-aggression 0 --ai-control 0 --ai-perfection 0 --ai-corner-entry-speed 0 --ai-corner-exit-speed 0 --ai-start-line 0
        ```
    - To make the game as hard as possible, you can run
        ```bat
        Toca3DifficultyEditor --career-difficulty 100 --ai-aggression 1 --ai-control 1 --ai-perfection 1 --ai-corner-entry-speed 1 --ai-corner-exit-speed 1 --ai-start-line 1
        ```
    - `--career-difficulty` is an integer in the range [60, 100] \(lower is easier)
    - `--ai-aggression`, `--ai-control`, `--ai-perfection`, `--ai-corner-entry-speed`, `--ai-corner-exit-speed`, and `--ai-start-line` are floating-point numbers in the range [0, 1] \(lower is easier)
    - If the game's installation directory can't be automatically detected, you can move `Toca3DifficultyEditor.exe` to the game installation directory, or pass the `--game-dir` argument.
        ```bat
        Toca3DifficultyEditor --game-dir "D:\Games\Race Driver 3" --career-difficulty 60
        ```
1. Start ToCA Race Driver 3.