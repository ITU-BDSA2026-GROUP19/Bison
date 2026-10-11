using Microsoft.EntityFrameworkCore;

public class PostRepository : IPostRepository
{
    private readonly BisonDBContext _context;

    public PostRepository(BisonDBContext context)
    {
        _context = context;
    }

    public async Task<List<Observation>> GetPostsByPageAsync(int pageNumber, int pageSize)
    {
        return await _context.Posts
            .OfType<Observation>()
            .OrderByDescending(p => p.TimeStamp)
            .ThenByDescending(p => p.PostId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(p => p.Author)
            .ToListAsync();
    }

    public async Task<List<Observation>> GetPostsByAuthorAsync(string authorName, int pageNumber, int pageSize)
    {
        return await _context.Posts
            .OfType<Observation>()
            .Where(p => p.Author.Name == authorName)
            .OrderByDescending(p => p.TimeStamp)
            .ThenByDescending(p => p.PostId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(p => p.Author)
            .ToListAsync();
    }


    public async Task<Observation?> GetObservationByIdAsync(int postId)
    {
        return await _context.Posts
            .OfType<Observation>()
            .Include(p => p.Author)
            .FirstOrDefaultAsync(p => p.PostId == postId);
    }

    public async Task<List<Comment>> GetCommentsByObservationIdAsync(int observationId)
    {
        return await _context.Posts
            .OfType<Comment>()
            .Where(c => c.Observation.PostId == observationId)
            .OrderBy(c => c.TimeStamp)
            .Include(c => c.Author)
            .ToListAsync();
    }

    public async Task<List<Proposal>> GetProposalsByObservationIdAsync(int observationId)
    {
        return await _context.Posts
            .OfType<Proposal>()
            .Where(p => p.Observation.PostId == observationId)
            .OrderBy(p => p.TimeStamp)
            .Include(p => p.Author)
            .Include(p => p.Taxon)
            .ToListAsync();
    }

    public async Task AddPostAsync(Post post)
    {
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePostAsync(Post post)
    {
        _context.Posts.Update(post);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePostAsync(int postId)
    {
        var post = await _context.Posts.FindAsync(postId);
        if (post != null)
        {
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }
    }
}