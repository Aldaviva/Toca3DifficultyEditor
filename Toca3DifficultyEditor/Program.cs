using McMaster.Extensions.CommandLineUtils;
using Microsoft.Win32;
using System.Text;
using System.Text.RegularExpressions;
using Toca3DifficultyEditor;

const string CHAMP_BIG_FILE_PATH_RELATIVE_TO_GAMEDIR = @"gamedata\chamship\champ.big";
const string MODS_FILE_PATH_RELATIVE_TO_GAMEDIR      = @"gamedata\frontend\Mods.ini";

Encoding fileEncoding = Encoding.Latin1;
var optionsParser = new CommandLineApplication<Options> {
    UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.Throw,
    Description                  = "Change the difficulty of all AI drivers in ToCA Race Driver 3"
};

optionsParser.Conventions.UseDefaultConventions();
optionsParser.ExtendedHelpText =
    $"""

     Examples:
       Set all of the AI drivers to be as easy as the easiest stock AI driver:
         {optionsParser.Name} --career-difficulty 80 --ai-aggression 0.6 --ai-control 0.5 --ai-perfection 0.5 --ai-corner-entry-speed 0.6 --ai-corner-exit-speed 0.4 --ai-start-line 0.5
         
       Set all of the AI drivers to minimum difficulty:
         {optionsParser.Name} --career-difficulty 60 --ai-aggression 0 --ai-control 0 --ai-perfection 0 --ai-corner-entry-speed 0 --ai-corner-exit-speed 0 --ai-start-line 0
         
       Set all of the AI drivers to maximum difficulty:
         {optionsParser.Name} --career-difficulty 100 --ai-aggression 1 --ai-control 1 --ai-perfection 1 --ai-corner-entry-speed 1 --ai-corner-exit-speed 1 --ai-start-line 1
         
       Specify the game installation directory if it can't be found automatically:
         {optionsParser.Name} --game-dir "C:\Program Files (x86)\Codemasters\Race Driver 3" --career-difficulty 60
     """;

try {
    optionsParser.Parse(args);
} catch (CommandParsingException e) {
    Console.WriteLine($"Invalid arguments: {e.Message}");
    optionsParser.ShowHelp();
    return 1;
}

Options options = optionsParser.Model;
bool shouldChangeChampBigFile = options.aiAggression != null
    || options.aiControl != null
    || options.aiCornerEntrySpeed != null
    || options.aiCornerExitSpeed != null
    || options.aiPerfection != null
    || options.aiStartLine != null;
if (optionsParser.OptionHelp?.HasValue() ?? false) {
    return 0;
} else if (!shouldChangeChampBigFile && options.careerDifficulty == null) {
    optionsParser.ShowHelp();
    return 0;
}

CancellationTokenSource cts = new();
CancellationToken       ct  = cts.Token;
Console.CancelKeyPress += (_, eventArgs) => {
    cts.Cancel();
    eventArgs.Cancel = true;
};

string? gameInstallationDirectory = findGameInstallationDirectory();
if (gameInstallationDirectory == null) {
    Console.WriteLine("Could not find ToCA Race Driver 3 installation directory.");
    Console.WriteLine(@"Try running this program with the command line option --game-dir ""C:\Path\To\Race Driver 3""");
    return 1;
}

if (shouldChangeChampBigFile) {
    string champBigFilePath     = Path.Combine(gameInstallationDirectory, CHAMP_BIG_FILE_PATH_RELATIVE_TO_GAMEDIR);
    byte[] champBigFileContents = await File.ReadAllBytesAsync(champBigFilePath, ct);

    IDictionary<string, double?> changesToMake = new Dictionary<string, double?> {
        ["AIAggression = "] = options.aiAggression,
        ["AIControl = "]    = options.aiControl,
        ["AIMistakes = "]   = options.aiPerfection,
        ["AISpeedIn = "]    = options.aiCornerEntrySpeed,
        ["AISpeedOut = "]   = options.aiCornerExitSpeed,
        ["AIStartline = "]  = options.aiStartLine
    };

    foreach (KeyValuePair<string, double> changeToMake in changesToMake.Compact()) {
        setAllValues(champBigFileContents, changeToMake.Key, changeToMake.Value.ToString("F5"));
    }

    await File.WriteAllBytesAsync(champBigFilePath, champBigFileContents, ct);
}

if (options.careerDifficulty != null) {
    string modsFilePath = Path.Combine(gameInstallationDirectory, MODS_FILE_PATH_RELATIVE_TO_GAMEDIR);
    string modsContents = await File.ReadAllTextAsync(modsFilePath, fileEncoding, ct);

    modsContents = Regex.Replace(modsContents, @"^CareerDiff\s*=\s*([\d\.,+-]+)(?=\r?$)", $"CareerDiff={options.careerDifficulty:D}", RegexOptions.Multiline);

    await File.WriteAllTextAsync(modsFilePath, modsContents, fileEncoding, ct);
}

return ct.IsCancellationRequested ? 2 : 0;

string? findGameInstallationDirectory() {
    if (options.gameDirectory != null) {
        if (isCorrectGameDirectory(options.gameDirectory)) {
            return options.gameDirectory;
        } else {
            Console.WriteLine($"ToCA Race Driver 3 installation not found in {options.gameDirectory}");
            return null;
        }
    }

    // Default installation director is C:\Program Files (x86)\Codemasters\Race Driver 3
    string defaultInstallationDirectory = Environment.ExpandEnvironmentVariables(@"%ProgramFiles(x86)%\Codemasters\Race Driver 3");
    if (isCorrectGameDirectory(defaultInstallationDirectory)) {
        return defaultInstallationDirectory;
    }

    if (isCorrectGameDirectory(Environment.CurrentDirectory)) {
        return Environment.CurrentDirectory;
    }

    if (Path.GetDirectoryName(Environment.ProcessPath) is { } selfExeDir && isCorrectGameDirectory(selfExeDir)) {
        return selfExeDir;
    }

    using RegistryKey   wowUninstallKey  = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall")!;
    IEnumerable<string> uninstallSubKeys = wowUninstallKey.GetSubKeyNames();
    foreach (string? child in uninstallSubKeys) {
        using RegistryKey uninstaller = wowUninstallKey.OpenSubKey(child)!;
        if (uninstaller.GetValue("DisplayName") as string == "Race Driver 3"
            && uninstaller.GetValue("Publisher") as string == "Codemasters"
            && uninstaller.GetValue("InstallLocation") is string installationLocation
            && isCorrectGameDirectory(installationLocation)) {
            return installationLocation;
        }
    }

    return null;

    static bool isCorrectGameDirectory(string dir) => File.Exists(Path.Combine(dir, "rd3.exe"))
        && File.Exists(Path.Combine(dir, CHAMP_BIG_FILE_PATH_RELATIVE_TO_GAMEDIR))
        && File.Exists(Path.Combine(dir, MODS_FILE_PATH_RELATIVE_TO_GAMEDIR));
}

void setAllValues(byte[] fileContents, string query, string newValue) {
    byte[] newValueBytes = newValue.ToBytes(fileEncoding);
    for (int start = 0; start < fileContents.Length - query.Length - newValueBytes.Length; start++) {
        if (fileEncoding.GetString(fileContents, start, query.Length) == query) {
            newValueBytes.CopyTo(fileContents, start + query.Length);
            start += query.Length + newValueBytes.Length - 1;
        }
    }
}