public class Observation : Post
{
    public Taxon Taxon { get; set; }
    public List<Comment> Comments { get; set; } = new List<Comment>();
    public List<Proposal> Proposals { get; set; } = new List<Proposal>();
}