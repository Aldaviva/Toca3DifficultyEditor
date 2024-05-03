using McMaster.Extensions.CommandLineUtils;
using System.ComponentModel.DataAnnotations;

namespace Toca3DifficultyEditor;

public class Options {

    [Option("--career-difficulty <DIFF>", "Change overall difficulty. [60, 100] optional, but seems to be needed to make the other options take effect, lower is easier",
        CommandOptionType.SingleValue)]
    [Range(60, 100)]
    public int? careerDifficulty { get; set; }

    [Option("--ai-aggression <X>", "Change the AI's aggression. [0.0, 1.0] optional, lower is easier", CommandOptionType.SingleValue)]
    [Range(0, 1)]
    public double? aiAggression { get; set; }

    [Option("--ai-control <X>", "Change the AI's car control. [0.0, 1.0] optional, lower is easier", CommandOptionType.SingleValue)]
    [Range(0, 1)]
    public double? aiControl { get; set; }

    [Option("--ai-perfection <X>", "Change the AI's mistake avoidance. [0.0, 1.0] optional, lower is easier", CommandOptionType.SingleValue)]
    [Range(0, 1)]
    public double? aiPerfection { get; set; }

    [Option("--ai-corner-entry-speed <X>", "Change the AI's speed entering corners. [0.0, 1.0] optional, lower is easier", CommandOptionType.SingleValue)]
    [Range(0, 1)]
    public double? aiCornerEntrySpeed { get; set; }

    [Option("--ai-corner-exit-speed <X>", "Change the AI's speed exiting corners. [0.0, 1.0] optional, lower is easier", CommandOptionType.SingleValue)]
    [Range(0, 1)]
    public double? aiCornerExitSpeed { get; set; }

    [Option("--ai-start-line <X>", "Change the AI's adherence to the ideal racing line. [0.0, 1.0] optional, lower is easier", CommandOptionType.SingleValue)]
    [Range(0, 1)]
    public double? aiStartLine { get; set; }

    [Option("--game-dir <DIR>", @"Game installation directory if it can't be autodetected, for example ""C:\Program Files (x86)\Codemasters\Race Driver 3"".", CommandOptionType.SingleValue)]
    public string? gameDirectory { get; set; }

}