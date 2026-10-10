public class Taxon
{
    public string TaxonId { get; set; }
    public string VernacularName { get; set; }
    public Taxon? Parent { get; set; }
    public List<Taxon> Children { get; set; } = new List<Taxon>();
}