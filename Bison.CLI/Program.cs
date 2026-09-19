using System.CommandLine;
using Bison.Cheep;
using SimpleDB;

CSVDatabase<Cheep> database = new CSVDatabase<Cheep>("bison_observe_cli_db.csv");

CSVDatabase<Observation> observationsDatabase = new CSVDatabase<Observation>("bison_observations.csv");

CSVDatabase<Comment> commentsDatabase = new CSVDatabase<Comment>("bison_comments.csv");

// --- "read" command ---
var readCommand = new Command("read", "Read all observations");
readCommand.SetAction((_) =>
{
    UserInterface.PrintObservations(observationsDatabase.Read());
});

// --- "observe" command ---
var observeMessageArg = new Argument<string>("message") { Description = "The observation message" };
var observeCommand = new Command("observe", "Store a new observation");
observeCommand.Arguments.Add(observeMessageArg);
observeCommand.SetAction((result) =>
{
    string message = result.GetValue(observeMessageArg)!;
    int nextId = observationsDatabase.Read().Select(observation => observation.Id).DefaultIfEmpty(0).Max() + 1;
    Observation observation = new Observation(nextId, Environment.UserName, message, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    observationsDatabase.Store(observation);
});

// --- "comment" command ---
var commentObservationIdArg = new Argument<int>("observation-id") { Description = "The ID of the observation to comment on" };
var commentMessageArg = new Argument<string>("message") { Description = "The comment message" };
var commentCommand = new Command("comment", "Add a comment to an observation");
commentCommand.Arguments.Add(commentObservationIdArg);
commentCommand.Arguments.Add(commentMessageArg);
commentCommand.SetAction((result) =>
{
    int observationId = result.GetValue(commentObservationIdArg);
    string message = result.GetValue(commentMessageArg)!;
    bool observationExists = observationsDatabase.Read().Any(observation => observation.Id == observationId);

    if (!observationExists)
    {
        Console.WriteLine($"Observation with ID {observationId} does not exist.");
        return;
    }

    Comment comment = new Comment(observationId, Environment.UserName, message, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    commentsDatabase.Store(comment);
});

// --- "discussion" command ---
var discussionObservationIdArg = new Argument<int>("observation-id") { Description = "The ID of the observation" };
var discussionCommand = new Command("discussion", "Read comments for an observation");
discussionCommand.Arguments.Add(discussionObservationIdArg);
discussionCommand.SetAction((result) =>
{
    int observationId = result.GetValue(discussionObservationIdArg);
    IEnumerable<Comment> comments = commentsDatabase.Read().Where(comment => comment.ObservationId == observationId);

    UserInterface.PrintComments(comments);
});

// --- root command ---
var rootCommand = new RootCommand("Bison CLI - observe and read cheeps");
rootCommand.Subcommands.Add(readCommand);
rootCommand.Subcommands.Add(observeCommand);
rootCommand.Subcommands.Add(commentCommand);
rootCommand.Subcommands.Add(discussionCommand);

return rootCommand.Parse(args).Invoke();