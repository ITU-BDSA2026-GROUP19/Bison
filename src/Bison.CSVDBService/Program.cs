using SimpleDB;
using Bison.Cheep;
using System.Reflection;
using System.Globalization;
using CsvHelper;

// Making the databases
CSVDatabase<Observation> observationsDatabase = CSVDatabase<Observation>.GetInstance("../../data/bison_observations.csv", new ObservationMap());

CSVDatabase<Comment> commentsDatabase = CSVDatabase<Comment>.GetInstance("../../data/bison_comments.csv");

CSVDatabase<Proposal> proposalsDatabase = CSVDatabase<Proposal>.GetInstance("../../data/bison_proposals.csv");


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


var assembly = Assembly.GetEntryAssembly()
    ?? throw new InvalidOperationException("Could not find the entry assembly.");

var resourceStream = assembly.GetManifestResourceStream("Bison.CSVDBService.joined.csv")
    ?? throw new InvalidOperationException("Could not find embedded taxonomy resource.");

using StreamReader reader = new StreamReader(resourceStream);
using CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
csvReader.Context.RegisterClassMap(new TaxonMap());

List<Taxon> taxons = csvReader.GetRecords<Taxon>().ToList();
Dictionary<string, Taxon> taxonsById = taxons.ToDictionary(taxon => taxon.Id);
Dictionary<string, Taxon> taxonsByVernacularName = taxons.Where(taxon => !string.IsNullOrWhiteSpace(taxon.VernacularName)).ToDictionary(taxon => taxon.VernacularName!);

foreach (Taxon taxon in taxons)
{
    if (taxonsById.TryGetValue(taxon.ParentId, out Taxon? parent))
    {
        taxon.Parent = parent;
        parent.Children.Add(taxon);
    }
}

// get proposals with a certain observationId
app.MapGet("/proposals/{id}", (int id) => {
    return proposalsDatabase.Read().Where(proposal => proposal.ObservationId == id);
});

// post the proposal
app.MapPost("/proposal", (Proposal proposal) =>
{
    if (taxonsById.ContainsKey(proposal.TaxonId))
    {
        proposalsDatabase.Store(proposal);
    } else
    {
        throw new InvalidOperationException( "The given TaxonId does not exist.");
    }
});

app.Run();