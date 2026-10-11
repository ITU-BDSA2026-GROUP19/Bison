public class Taxon
{
    public string dwc_TaxonID { get; set; }
    public int TaxonId { get; set; }
    public string? VernacularName { get; set; }
    public Taxon? Parent { get; set; }
    public List<Taxon> Children { get; set; } = new List<Taxon>();
}