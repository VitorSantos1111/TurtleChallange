// See https://aka.ms/new-console-template for more information

// Args check.
if (args.Length == 0)
{
    // Invalid number of arguments.
    PrintUsage();
}
else if (args[0].StartsWith("-"))
{
    // User wants to save a example file.
    switch (args[0])
    {
        // Settings example file.
        case "-s":
            SaveSettingsExample((args.Length > 2)
                ? args[1]
                : "");
            break;

        // Actions example file.
        case "-m":
            SaveActionsExample((args.Length > 2)
                ? args[1]
                : "");
            break;
        default:
            PrintUsage();
            break;
    }
}
else
{
    // Assume game.
    StartGame(args[0], args[1]);
}


/* 
* Internal methods.
*/

/*
* Print usage.
*/
static void PrintUsage()
{
    var programName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

    Console.WriteLine("Turtle game options:");
    Console.WriteLine(programName + " <settings file> <moves file>: Runs the game with the given settings and moves.");
    Console.WriteLine(programName + " -s <settings file>: Saves a settings file example to the given path.");
    Console.WriteLine(programName + " -m <moves file>: Saves a moves file example to the given path.");
    Console.WriteLine();
}

/*
* Runs the game.
*/
static void StartGame(string serviceFilePath, string actionsFilePath)
{
    try
    {
        // Load the files.
        var loadedSettings = Services.SettingsService.LoadFromFile(serviceFilePath);
        var loadedActions = Services.ActionsServices.LoadFromFile(actionsFilePath);
    
        // Start the game engine.
        var gameEngine = new Services.GameEngine(loadedSettings, loadedActions);
        Console.WriteLine();
        gameEngine.Run();
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        // Output to the error pipe.
        Console.Error.WriteLine(string.Format("Could not start the game: {0}", ex.Message));
    }
}

/*
* Save an example settings file to the given path.
*/
static void SaveSettingsExample(string path)
{
    // Replace path to default one if none given.
    if (string.IsNullOrWhiteSpace(path))
    {
        path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Environment.CommandLine), "settings.json");
    }

    // Create a dummy settings.
    var s = new Models.Settings
    {
        Rows = 4,
        Columns = 5,
        InitialDirection = Direction.North,
        StartingTile = new Models.Tile(0, 1),
        ExitTile = new Models.Tile(4, 2),
        Mines = new List<Models.Tile>
        { 
            new Models.Tile(1, 1),
            new Models.Tile(3, 1),
            new Models.Tile(3, 3)
        }
    };

    // Save the settings to the given path.
    Services.SettingsService.SaveToFile(path, s);
    Console.WriteLine(string.Format("Settings saved to {0}.", path));
}

/*
* Save an example actions file to the given path.
*/
static void SaveActionsExample(string path)
{
    // Replace path to default one if none given.
    if (string.IsNullOrWhiteSpace(path))
    {
        path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Environment.CommandLine), "actions.csv");
    }

    // Create a dummy actions.
    var actions = new List<Actions>
    {
        Actions.Move,
        Actions.Rotate,
        Actions.Move,
        Actions.Move,
        Actions.Move,
        Actions.Move,
        Actions.Rotate,
        Actions.Move,
        Actions.Move
    };

    // Save the actions to the given path.
    Services.ActionsServices.SaveToFile(path, actions);
    Console.WriteLine(string.Format("Actions saved to {0}.", path));
}