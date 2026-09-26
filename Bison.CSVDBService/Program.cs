using SimpleDB;
using Bison.Cheep;

// Making the databases
CSVDatabase<Observation> observationsDatabase = CSVDatabase<Observation>.GetInstance("../Bison.CLI/bison_observations.csv", new ObservationMap());

CSVDatabase<Comment> commentsDatabase = CSVDatabase<Comment>.GetInstance("../Bison.CLI/bison_comments.csv");


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Read from the observations CSV
app.MapGet("/observations", () => observationsDatabase.Read());

// get comments with a certain id
app.MapGet("/comments/{id}", (int id) => {
    return commentsDatabase.Read().Where(comment => comment.ObservationId == id);
});

// post the observation
app.MapPost("/observation", (Observation observation) => observationsDatabase.Store(observation));

// post the comment
app.MapPost("/comment", (Comment comment) => commentsDatabase.Store(comment));

app.Run();