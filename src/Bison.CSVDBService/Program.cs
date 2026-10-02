using SimpleDB;
using Bison.Cheep;

// The service uses the same CSV files as the CLI application.
CSVDatabase<Observation> observationsDatabase = CSVDatabase<Observation>.
    GetInstance("../../data/bison_observations.csv", new ObservationMap());

CSVDatabase<Comment> commentsDatabase = CSVDatabase<Comment>.GetInstance("../../data/bison_comments.csv");

CSVDatabase<Proposal> proposalsDatabase = CSVDatabase<Proposal>.GetInstance("../../data/bison_proposals.csv");

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


app.MapGet("/observations", () => observationsDatabase.Read());


app.MapGet("/comments/{id}", (int id) =>
{
    return commentsDatabase.Read()
        .Where(comment => comment.ObservationId == id);
});


app.MapPost("/observation", (Observation observation) =>
    observationsDatabase.Store(observation));

app.MapPost("/comment", (Comment comment) =>
    commentsDatabase.Store(comment));

// Load the embedded taxonomy so proposals can be validated against known taxon IDs.
Taxonomy taxonomy = Taxonomy.Load();

app.MapGet("/proposals/{id}", (int id) =>
{
    return proposalsDatabase.Read()
        .Where(proposal => proposal.ObservationId == id);
});


// A proposal is only stored if its taxon ID exists in the loaded taxonomy.
app.MapPost("/proposal", (Proposal proposal) =>
{
    if (taxonomy.GetById(proposal.TaxonId) != null)
    {
        proposalsDatabase.Store(proposal);
    }
    else
    {
        throw new InvalidOperationException("The given TaxonId does not exist.");
    }
});

app.Run();