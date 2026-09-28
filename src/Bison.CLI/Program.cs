using System.CommandLine;
using Bison.Cheep;
using System.Net.Http.Json;
using SimpleDB;

//CSVDatabase<Cheep> database = CSVDatabase<Cheep>.GetInstance("../../data/bison_observe_cli_db.csv");

CSVDatabase<Observation> observationsDatabase = CSVDatabase<Observation>.GetInstance("../../data/bison_observations.csv", new ObservationMap());

CSVDatabase<Comment> commentsDatabase = CSVDatabase<Comment>.GetInstance("../../data/bison_comments.csv");

HttpClient client = new HttpClient
{
    BaseAddress = new Uri("http://localhost:5273")
};

// --- "read" command ---
var readCommand = new Command("read", "Read all observations");
readCommand.SetAction(async (_) =>
{
    List<Observation> observations =
    await client.GetFromJsonAsync<List<Observation>>("/observations")
    ?? [];
    UserInterface.PrintObservations(observations);
});

// --- "observe" command ---
var observeMessageArg = new Argument<string>("message") { Description = "The observation message" };
var observationLocationArg = new Argument<string>("location") { Description = "The location of the observation" };
var observeCommand = new Command("observe", "Store a new observation");
observeCommand.Arguments.Add(observeMessageArg);
observeCommand.Arguments.Add(observationLocationArg);
observeCommand.SetAction(async (result) =>
{
    string message = result.GetValue(observeMessageArg)!;
    string location = result.GetValue(observationLocationArg)!;

    List<Observation> observations =
    await client.GetFromJsonAsync<List<Observation>>("/observations")
    ?? [];

    int nextId = observations.Select(observation => observation.Id).DefaultIfEmpty(0).Max() + 1;
    Observation observation = new Observation(nextId, Environment.UserName, message, DateTimeOffset.UtcNow.ToUnixTimeSeconds(), location);
    await client.PostAsJsonAsync("/observation", observation);

    //observationsDatabase.Store(observation);
});

// --- "comment" command ---
var commentObservationIdArg = new Argument<int>("observation-id") { Description = "The ID of the observation to comment on" };
var commentMessageArg = new Argument<string>("message") { Description = "The comment message" };
var commentCommand = new Command("comment", "Add a comment to an observation");
commentCommand.Arguments.Add(commentObservationIdArg);
commentCommand.Arguments.Add(commentMessageArg);
commentCommand.SetAction(async (result) =>
{
    int observationId = result.GetValue(commentObservationIdArg);
    string message = result.GetValue(commentMessageArg)!;

    List<Observation> observations =
    await client.GetFromJsonAsync<List<Observation>>("/observations")
    ?? [];

    bool observationExists = observations.Any(observation => observation.Id == observationId);

    if (!observationExists)
    {
        Console.WriteLine($"Observation with ID {observationId} does not exist.");
        return;
    }

    Comment comment = new Comment(observationId, Environment.UserName, message, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    await client.PostAsJsonAsync("/comment", comment);
    //commentsDatabase.Store(comment);
});

// --- "discussion" command ---
var discussionObservationIdArg = new Argument<int>("observation-id") { Description = "The ID of the observation" };
var discussionCommand = new Command("discussion", "Read comments for an observation");
discussionCommand.Arguments.Add(discussionObservationIdArg);
discussionCommand.SetAction(async (result) =>
{
    int observationId = result.GetValue(discussionObservationIdArg);

    List<Comment> comments =
    await client.GetFromJsonAsync<List<Comment>>($"/comments/{observationId}")
    ?? [];

    //IEnumerable<Comment> comments = commentsDatabase.Read().Where(comment => comment.ObservationId == observationId);

    UserInterface.PrintComments(comments);
});


// --- "location" command ---
var locationArg = new Argument<string>("location") { Description = "The location of the observations" };
var locationCommand = new Command("location", "Read observations by location");
locationCommand.Arguments.Add(locationArg);
locationCommand.SetAction((result) =>
{
    string location = result.GetValue(locationArg)!;
    IEnumerable<Observation> observations = observationsDatabase.Read().Where(observation => observation.Location == location);
    UserInterface.PrintObservations(observations);
});

// --- root command ---
var rootCommand = new RootCommand("Bison CLI - observe and read cheeps");
rootCommand.Subcommands.Add(readCommand);
rootCommand.Subcommands.Add(observeCommand);
rootCommand.Subcommands.Add(commentCommand);
rootCommand.Subcommands.Add(discussionCommand);
rootCommand.Subcommands.Add(locationCommand);

return rootCommand.Parse(args).Invoke();