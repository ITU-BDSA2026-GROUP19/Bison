namespace Bison.Cheep;

public record Taxon(string Id, string ParentId, string Rank, string ScientificName, string VernacularName)
{
    public Taxon? Parent { get; set; }
    public List<Taxon> Children { get; } = [];
}