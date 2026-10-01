using CsvHelper;
using System.Globalization;
using System.Reflection;

namespace Bison.Cheep;

public class Taxonomy
{
    private readonly Dictionary<string, Taxon> taxonsById;
    private readonly Dictionary<string, Taxon> taxonsByVernacularName;

    private Taxonomy(IEnumerable<Taxon> taxons)
    {
        // Keep separate lookups for taxon IDs and Danish vernacular names.
        this.taxonsById = taxons.ToDictionary(taxon => taxon.Id);

        this.taxonsByVernacularName = taxons
            .Where(taxon => !string.IsNullOrWhiteSpace(taxon.VernacularName))
            .ToDictionary(taxon => taxon.VernacularName!);

        // Build the taxonomy tree from the parent IDs in the CSV data.
        foreach (Taxon taxon in taxons)
        {
            if (taxonsById.TryGetValue(taxon.ParentId, out Taxon? parent))
            {
                taxon.Parent = parent;
                parent.Children.Add(taxon);
            }
        }
    }

    public static Taxonomy Load()
    {
        // The taxonomy is embedded in the service assembly so it is available
        // when the application starts without requiring a separate CSV file.
        var assembly = Assembly.GetEntryAssembly()
            ?? throw new InvalidOperationException("Could not find the entry assembly.");

        var resourceStream = assembly.GetManifestResourceStream("Bison.CSVDBService.joined.csv")
            ?? throw new InvalidOperationException("Could not find embedded taxonomy resource.");

        using StreamReader reader = new(resourceStream);
        using CsvReader csvReader = new(reader, CultureInfo.InvariantCulture);

        csvReader.Context.RegisterClassMap(new TaxonMap());

        List<Taxon> taxons = csvReader.GetRecords<Taxon>().ToList();

        return new Taxonomy(taxons);
    }

    public Taxon? GetById(string id)
    {
        taxonsById.TryGetValue(id, out Taxon? taxon);
        return taxon;
    }

    public Taxon? GetByVernacularName(string name)
    {
        taxonsByVernacularName.TryGetValue(name, out Taxon? taxon);
        return taxon;
    }
}