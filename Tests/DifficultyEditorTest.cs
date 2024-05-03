using Microsoft.Win32;
using System.Reflection;

namespace Tests;

public class DifficultyEditorTest {

    private static Task<int> main(params string[] args) {
        MethodInfo mainMethod = typeof(Program).GetMethod("<Main>$", BindingFlags.Static | BindingFlags.NonPublic, [typeof(string[])])!;

        return (Task<int>) mainMethod.Invoke(null, [args])!;
    }

    public DifficultyEditorTest() {
        File.Copy(@"Game\Codemasters\Race Driver 3\gamedata\frontend\Mods.ini.original", @"Game\Codemasters\Race Driver 3\gamedata\frontend\Mods.ini", true);
        File.Copy(@"Game\Codemasters\Race Driver 3\gamedata\chamship\champ.big.original", @"Game\Codemasters\Race Driver 3\gamedata\chamship\champ.big", true);
    }

    [Fact]
    public async Task minimumDifficulty() {
        int exitCode = await main(
            "--game-dir", @"Game\Codemasters\Race Driver 3",
            "--career-difficulty", "60",
            "--ai-aggression", "0",
            "--ai-control", "0",
            "--ai-perfection", "0",
            "--ai-corner-entry-speed", "0",
            "--ai-corner-exit-speed", "0",
            "--ai-start-line", "0"
        );

        exitCode.Should().Be(0);
        await fileContentsShouldBeEqual(@"Game\Codemasters\Race Driver 3\gamedata\frontend\Mods.ini", @"Game\Codemasters\Race Driver 3\gamedata\frontend\Mods.ini.mindifficulty");
        await fileContentsShouldBeEqual(@"Game\Codemasters\Race Driver 3\gamedata\chamship\champ.big", @"Game\Codemasters\Race Driver 3\gamedata\chamship\champ.big.mindifficulty");
    }

    [Fact]
    public async Task maximumDifficulty() {
        int exitCode = await main(
            "--game-dir", @"Game\Codemasters\Race Driver 3",
            "--career-difficulty", "100",
            "--ai-aggression", "1",
            "--ai-control", "1",
            "--ai-perfection", "1",
            "--ai-corner-entry-speed", "1",
            "--ai-corner-exit-speed", "1",
            "--ai-start-line", "1"
        );

        exitCode.Should().Be(0);
        await fileContentsShouldBeEqual(@"Game\Codemasters\Race Driver 3\gamedata\frontend\Mods.ini", @"Game\Codemasters\Race Driver 3\gamedata\frontend\Mods.ini.maxdifficulty");
        await fileContentsShouldBeEqual(@"Game\Codemasters\Race Driver 3\gamedata\chamship\champ.big", @"Game\Codemasters\Race Driver 3\gamedata\chamship\champ.big.maxdifficulty");
    }

    [Fact]
    public async Task invalidArgument() {
        int exitCode = await main("--hargle");

        exitCode.Should().Be(1);
        await fileContentsShouldBeEqual(@"Game\Codemasters\Race Driver 3\gamedata\frontend\Mods.ini", @"Game\Codemasters\Race Driver 3\gamedata\frontend\Mods.ini.original");
        await fileContentsShouldBeEqual(@"Game\Codemasters\Race Driver 3\gamedata\chamship\champ.big", @"Game\Codemasters\Race Driver 3\gamedata\chamship\champ.big.original");
    }

    [Fact]
    public async Task showHelp() {
        int exitCode = await main("--help");

        exitCode.Should().Be(0);
        await fileContentsShouldBeEqual(@"Game\Codemasters\Race Driver 3\gamedata\frontend\Mods.ini", @"Game\Codemasters\Race Driver 3\gamedata\frontend\Mods.ini.original");
        await fileContentsShouldBeEqual(@"Game\Codemasters\Race Driver 3\gamedata\chamship\champ.big", @"Game\Codemasters\Race Driver 3\gamedata\chamship\champ.big.original");
    }

    [Fact]
    public async Task noArguments() {
        int exitCode = await main();

        exitCode.Should().Be(0);
        await fileContentsShouldBeEqual(@"Game\Codemasters\Race Driver 3\gamedata\frontend\Mods.ini", @"Game\Codemasters\Race Driver 3\gamedata\frontend\Mods.ini.original");
        await fileContentsShouldBeEqual(@"Game\Codemasters\Race Driver 3\gamedata\chamship\champ.big", @"Game\Codemasters\Race Driver 3\gamedata\chamship\champ.big.original");
    }

    [Fact]
    public async Task wrongGameDir() {
        int exitCode = await main(
            "--game-dir", "Game-wrong",
            "--career-difficulty", "60");

        exitCode.Should().Be(1);
        await fileContentsShouldBeEqual(@"Game\Codemasters\Race Driver 3\gamedata\frontend\Mods.ini", @"Game\Codemasters\Race Driver 3\gamedata\frontend\Mods.ini.original");
        await fileContentsShouldBeEqual(@"Game\Codemasters\Race Driver 3\gamedata\chamship\champ.big", @"Game\Codemasters\Race Driver 3\gamedata\chamship\champ.big.original");
    }

    // This only works if the game is not actually installed on the testing machine with the default name in Add/Remove Programs
    [Fact]
    public async Task missingGameDir() {
        int exitCode = await main("--career-difficulty", "60");

        exitCode.Should().Be(1);
        await fileContentsShouldBeEqual(@"Game\Codemasters\Race Driver 3\gamedata\frontend\Mods.ini", @"Game\Codemasters\Race Driver 3\gamedata\frontend\Mods.ini.original");
        await fileContentsShouldBeEqual(@"Game\Codemasters\Race Driver 3\gamedata\chamship\champ.big", @"Game\Codemasters\Race Driver 3\gamedata\chamship\champ.big.original");
    }

    [Fact]
    public async Task registry() {
        const string UNINSTALL_KEY_NAME = "Toca3DifficultyEditor-test";

        using RegistryKey uninstallKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall", true)!;
        using RegistryKey uninstallKey  = uninstallKeys.CreateSubKey(UNINSTALL_KEY_NAME, true);
        uninstallKey.SetValue("DisplayName", "Race Driver 3");
        uninstallKey.SetValue("Publisher", "Codemasters");
        uninstallKey.SetValue("InstallLocation", Path.Combine(Environment.CurrentDirectory, @"Game\Codemasters\Race Driver 3"));

        try {
            int exitCode = await main(
                "--career-difficulty", "60");

            exitCode.Should().Be(0);
            await fileContentsShouldBeEqual(@"Game\Codemasters\Race Driver 3\gamedata\frontend\Mods.ini", @"Game\Codemasters\Race Driver 3\gamedata\frontend\Mods.ini.mindifficulty");
            await fileContentsShouldBeEqual(@"Game\Codemasters\Race Driver 3\gamedata\chamship\champ.big", @"Game\Codemasters\Race Driver 3\gamedata\chamship\champ.big.original");
        } finally {
            uninstallKeys.DeleteSubKeyTree(UNINSTALL_KEY_NAME);
        }
    }

    [Fact]
    public async Task cwd() {
        string oldCwd = Environment.CurrentDirectory;
        try {
            Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, @"Game\Codemasters\Race Driver 3");
            int exitCode = await main("--career-difficulty", "60");
            exitCode.Should().Be(0);
            await fileContentsShouldBeEqual(@"gamedata\frontend\Mods.ini", @"gamedata\frontend\Mods.ini.mindifficulty");
        } finally {
            Environment.CurrentDirectory = oldCwd;
        }
    }

    [Fact]
    public async Task exeDir() {
        string    oldProcessPath   = Environment.ProcessPath!;
        FieldInfo processPathField = typeof(Environment).GetField("s_processPath", BindingFlags.NonPublic | BindingFlags.Static)!;
        try {
            processPathField.SetValue(null, Path.Combine(Environment.CurrentDirectory, @"Game\Codemasters\Race Driver 3", "fake-Toca3DifficultyEditor.exe"));

            int exitCode = await main("--career-difficulty", "60");

            exitCode.Should().Be(0);
            await fileContentsShouldBeEqual(@"Game\Codemasters\Race Driver 3\gamedata\frontend\Mods.ini", @"Game\Codemasters\Race Driver 3\gamedata\frontend\Mods.ini.mindifficulty");
        } finally {
            processPathField.SetValue(null, oldProcessPath);
        }
    }

    [Fact]
    public async Task defaultInstallationDirectory() {
        const string PROGRAM_FILES_ENV_VAR_NAME = "ProgramFiles(x86)";
        string       oldProgramFilesX86         = Environment.GetEnvironmentVariable(PROGRAM_FILES_ENV_VAR_NAME)!;
        try {
            Environment.SetEnvironmentVariable(PROGRAM_FILES_ENV_VAR_NAME, Path.Combine(Environment.CurrentDirectory, "Game"));
            int exitCode = await main("--career-difficulty", "60");
            exitCode.Should().Be(0);
            await fileContentsShouldBeEqual(@"Game\Codemasters\Race Driver 3\gamedata\frontend\Mods.ini", @"Game\Codemasters\Race Driver 3\gamedata\frontend\Mods.ini.mindifficulty");
        } finally {
            Environment.SetEnvironmentVariable(PROGRAM_FILES_ENV_VAR_NAME, oldProgramFilesX86);
        }
    }

    private static async Task fileContentsShouldBeEqual(string actualFilePath, string expectedFilePath) {
        (await File.ReadAllBytesAsync(actualFilePath)).Should().Equal(await File.ReadAllBytesAsync(expectedFilePath), Path.GetFileName(actualFilePath));
    }

}