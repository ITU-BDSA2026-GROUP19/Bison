namespace Bison.Core;

public class Observation : Post
{
    public Taxon Taxon { get; set; }
    public List<Comment> Comments { get; set; } = new List<Comment>();
    public List<Proposal> Proposals { get; set; } = new List<Proposal>();
    public string? PicturePath { get; set; } = null;
    public string? PictureLicense { get; set; } = null;

    public Taxon getTaxon ()
    {
        return Taxon;
    }
}