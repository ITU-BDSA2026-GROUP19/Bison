using System.CommandLine;
using Bison.Cheep;
using SimpleDB;

CSVDatabase<Cheep> database = new CSVDatabase<Cheep>("bison_observe_cli_db.csv");

// --- "read" command ---
var readCommand = new Command("read", "Read all cheeps");
readCommand.SetHandler(() =>
{
    foreach (Cheep record in database.Read())
    {
        DateTimeOffset date = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp);
        UserInterface.PrintCheeps(database.Read());
        return;
    }
    UserInterface.PrintCheeps(database.Read());
});

// --- "observe" command ---
var messageArg = new Argument<string>("message", "The observation message");
var observeCommand = new Command("observe", "Store a new observation");
observeCommand.AddArgument(messageArg);
observeCommand.SetHandler((string message) =>
{
    Cheep cheep = new Cheep(Environment.UserName, message, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    database.Store(cheep);
}, messageArg);

// --- root command ---
var rootCommand = new RootCommand("Bison CLI - observe and read cheeps");
rootCommand.AddCommand(readCommand);
rootCommand.AddCommand(observeCommand);

return await rootCommand.InvokeAsync(args);