using Microsoft.EntityFrameworkCore;

public class BisonDBContext : DbContext
{
    public BisonDBContext(DbContextOptions<BisonDBContext> options) : base(options)
    {
    }

    public DbSet<Post> Posts { get; set; }
    public DbSet<Proposal> Proposals { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Observation> Observations { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Taxon> Taxa { get; set; }
}