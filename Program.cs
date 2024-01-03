using GameRes;
using System.Diagnostics;
using System.Text.Json;

internal class Program
{
    static readonly string version = "v1.0.2";
    static readonly string copyrightYear = DateTime.Now.Year.ToString();
    static readonly string buildDate = "November 21th, 2023";
    static readonly string buildTime = "3:59 PM";
    static readonly string configFileLocation = "config.json";

    static RootConfig r = new()
    {
        Default = new()
        {
            Resolution = "native",
            Taskbar = false,
            Rotation = 0
        },
        Options = new()
        {
            Info = true,
            Network = true,
            Version = true
        },
        Data = [],
    };

    static void PrintHeader()
    {
        Console.Clear();

        Console.WriteLine($"GameRes {version} - (c) {copyrightYear} oscie57, SilverDiamond");

        char borderChar = '=';
        string borderLine = new(borderChar, Console.WindowWidth);
        Console.WriteLine(borderLine);

        Console.WriteLine();
    }

    static void ResetScreen()
    {
        PrintHeader(); // Prints the header

        Console.WriteLine("Reverting screen to normal..");

        var (x, y) = Utils.GetOptimalScreenResolution();
        var resX = (int)x;
        var resY = (int)y;

        Taskbar.Show();

        Console.WriteLine(PrmaryScreenResolution.ChangeResolution(resX, resY, r.Default.Rotation));
    }

    static void LaunchGame(string gamePath, int rotationValue, bool taskbar, string resolution, string type)
    {
        // Width, Height, Rotation
        int resX;
        int resY;

        switch (resolution)
        {
            case "720p":
                resX = 1280;
                resY = 720;
                break;
            case "768p":
                resX = 1366;
                resY = 768;
                break;
            case "1080p":
                resX = 1920;
                resY = 1080;
                break;
            default:
                var(x,y) = Utils.GetOptimalScreenResolution();
                resX = (int)x;
                resY = (int)y;
                break;
        }

        Console.WriteLine("Setting rotation and resolution...");
        Console.WriteLine(PrmaryScreenResolution.ChangeResolution(resX, resY, rotationValue));

        Console.WriteLine("Hiding taskbar...");
        Taskbar.Hide();

        Console.WriteLine("Launching... Enjoy your game!");
        Process spice = new();
        spice.StartInfo.FileName = gamePath;
        spice.StartInfo.UseShellExecute = true;
        spice.StartInfo.Verb = "runas";
        spice.StartInfo.WorkingDirectory = Path.GetDirectoryName(gamePath);
        spice.Start();
        spice.WaitForExit();
        Console.WriteLine("Game exited, resetting screen...");
        ResetScreen();
    }

    static bool SaveLoadConfig()
    {
        // if file exists skip this
        // otherwise make it !!!

        // once made, set default stuff using system values
        // then ask if its okay.


        if (File.Exists(configFileLocation))
        {
            var text = File.ReadAllText(configFileLocation);
            r = JsonSerializer.Deserialize<RootConfig>(text);
            return true;
        }
        else
        {
            SaveConfig();
            return false;
        }
    }

    static void SaveConfig()
    {
        JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };
        var text = JsonSerializer.Serialize(r, options);
        File.WriteAllText(configFileLocation, text);
    }

    static bool AskForBool(int stepN, string Question)
    {
        Console.WriteLine($"Step {stepN}. {Question} (y/n)");
        Console.Write(" -> ");
        return Console.ReadLine().ToLower() switch
        {
            "yes" or "y" => true,
            "no" or "n" => false,
            _ => AskForBool(stepN, Question),
        };
    }
    static int AskForInt(int stepN, string Question)
    {
        Console.WriteLine($"Step {stepN}. {Question}");
        Console.Write(" -> ");
        return int.TryParse(Console.ReadLine(), out var i) ? i : AskForInt(stepN, Question);
    }
    static string AskForString(int stepN, string Question, Func<string, bool> action)
    {
        Console.WriteLine($"Step {stepN}. {Question}");
        Console.Write(" -> ");
        var input = Console.ReadLine();

        if (action(input))
        {
            return input;
        }
        else
        {
            return AskForString(stepN, Question, action);
        }
    }

    static void SetupConfig()
    {
        PrintHeader();
        Console.WriteLine("Setup - Display options\n");
        r.Options.Network = AskForBool(1, "Would you like network information to be displayed?");
        r.Options.Info = AskForBool(2, "Would you like extra information to be displayed?");
        r.Options.Version = AskForBool(3, "Would you like the version to be displayed? ");
        SaveConfig();
    }

    static void AddGame()
    {
        PrintHeader();
        Console.WriteLine("Setup - Add game\n");
        string TempName = AskForString(1, "What is the title of the game? (i.e 'REFLEC BEAT VOLZZA 2')", (_)=>true);
        string TempPath = AskForString(2, "What is the path to the game? (i.e 'C:\\path\\to\\spice.exe')", Path.Exists);
        string TempCode = AskForString(3, "What is the game code? (i.e 'SDED' or 'MBR')", (x) =>
        {
            return x.Length is 3 or 4;
        });
        string TempVersion = AskForString(4, "What is the game version? (i.e '7.10.01' or '2023092000')", (_) => true);
        string TempInfo = AskForString(5, "What is the game info? (i.e 'English Omnimix')", (_) => true);
        string TempType = AskForString(6, "What is the game type? ('spice', 'segatools', or 'divaloader')", (x) => {
            List<string> allowed = ["spice", "segatools", "divaloader"];
            return allowed.Contains(x);
        });
        string TempNetwork = AskForString(7, "What network does the game run on? (i.e 'CGDev', 'BemaniSAC', 'None')", (_)=>true);
        int TempRotation = AskForInt(7, "What is the rotation value? (i.e '0', '90', '180', '270')");
        string TempResolution = AskForString(8, "What is the resolution? ('native', '720p', '768p', or '1080p')", (x) => {
            List<string> allowed = ["720p", "768p", "1080p", "native"];
            return allowed.Contains(x);
        });
        bool TempTaskbar = AskForBool(9, "Would you like the taskbar to be hidden?");

        r.Data.Add(new Game
        {
            Name = TempName,
            Path = TempPath.Trim('"'),
            Code = TempCode,
            Version = TempVersion,
            Info = TempInfo,
            Type = TempType,
            Network = TempNetwork,
            Rotation = TempRotation,
            Resolution = TempResolution,
            Taskbar = TempTaskbar
        });
        SaveConfig();
    }

    static void DeleteGame()
    {
        PrintHeader();

        Console.WriteLine("Setup - Remove game\n");

        for (int i = 0; i < r.Data.Count; i++)
        {
            Console.WriteLine($"{i}. {r.Data[i].Name} [{r.Data[i].Code} {r.Data[i].Version}]");
        }

        Console.WriteLine();
        Console.WriteLine("R. Return to setup menu");

        Console.Write(" -> ");
        string input = Console.ReadLine().ToLower();


        switch (input)
        {
            case "r":
                return;
            default:
                if (int.TryParse(input, out var i))
                {
                    r.Data.RemoveAt(i);
                    SaveConfig();
                    DeleteGame();
                }
                else
                {
                    Console.WriteLine("Invalid input.");
                    DeleteGame();
                }
                break;
        }
    }

    private static void Main(string[] args)
    {
        if (!SaveLoadConfig())
        {
            SetupConfig();
        }

        PrintHeader(); // Prints the header

        if (r.Data.Count == 0)
        {
            Console.WriteLine("No games found. Please add a game.");
        }
        else
        {
            for (int i = 0; i < r.Data.Count; i++)
            {
                bool showInfoBrackets = !string.IsNullOrWhiteSpace(r.Data[i].Info) && r.Options.Info;
                bool showVersionBrackets = !string.IsNullOrWhiteSpace(r.Data[i].Version) && r.Options.Version;
                bool showNetworkBrackets = !string.IsNullOrWhiteSpace(r.Data[i].Network) && r.Options.Network;
                Console.WriteLine($"{i}. {r.Data[i].Name}" +
                    $"{(showInfoBrackets?" (":null)}{(showInfoBrackets?r.Data[i].Info : "")}{(showInfoBrackets?')':null)}" +
                    $"{(showVersionBrackets?" [":null)}{(showVersionBrackets ? $"{r.Data[i].Code}-{r.Data[i].Version}" : "")}{(showVersionBrackets?']':null)}" +
                    $"{(showNetworkBrackets?" {":null)}{(showNetworkBrackets?r.Data[i].Network : "")}{(showNetworkBrackets?'}':null)}");
            }
        }
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("R. Reset to native resolution"); // runs ResetScreen();
        Console.WriteLine();
        Console.WriteLine("A. Add game"); // runs AddGame();
        Console.WriteLine("D. Delete game"); // runs DeleteGame();
        Console.WriteLine("O. Options"); // runs SetupConfig();
        Console.WriteLine();
        Console.WriteLine("E. Exit"); // exits the program

        Console.Write(" -> ");
        string input = Console.ReadLine().ToLower();

        switch (input)
        {
            case "r":
                ResetScreen();
                Main(args);
                break;
            case "a":
                AddGame();
                Main(args);
                break;
            case "d":
                DeleteGame();
                Main(args);
                break;
            case "o":
                SetupConfig();
                Main(args);
                break;
            case "e":
                Environment.Exit(0);
                break;
            default:
                if (int.TryParse(input, out var i))
                {
                    LaunchGame(r.Data[i].Path, r.Data[i].Rotation, r.Data[i].Taskbar, r.Data[i].Resolution, r.Data[i].Type);
                    Main(args);
                }
                else
                {
                    Console.WriteLine("Invalid input.");
                    Main(args);
                }
                break;
        }
    }
}