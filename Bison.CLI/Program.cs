using System.CommandLine;
using Bison.Cheep;
using SimpleDB;

CSVDatabase<Cheep> database = CSVDatabase<Cheep>.GetInstance("bison_observe_cli_db.csv");

// --- "read" command ---
var readCommand = new Command("read", "Read all cheeps");
readCommand.SetAction((_) =>
{
    UserInterface.PrintCheeps(database.Read());
});

// --- "observe" command ---
var messageArg = new Argument<string>("message") { Description = "The observation message" };
var observeCommand = new Command("observe", "Store a new observation");
observeCommand.Arguments.Add(messageArg);
observeCommand.SetAction((result) =>
{
    string message = result.GetValue(messageArg)!;
    Cheep cheep = new Cheep(Environment.UserName, message, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    database.Store(cheep);
});

// --- root command ---
var rootCommand = new RootCommand("Bison CLI - observe and read cheeps");
rootCommand.Subcommands.Add(readCommand);
rootCommand.Subcommands.Add(observeCommand);

return rootCommand.Parse(args).Invoke();