namespace Bison.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Bison.Core;

public class PostRepository : IPostRepository
{
    private readonly BisonDBContext _context;
    private const string DeletedUserName = "Deleted user";
    private const string DeletedUserEmail = "deleted-user@bison.local";


    public PostRepository(BisonDBContext context)
    {
        _context = context;
    }

    public async Task<List<Observation>> GetAllObservationsAsync()
    {
        return await _context.Posts
            .OfType<Observation>()
            .OrderByDescending(p => p.TimeStamp)
            .ThenByDescending(p => p.PostId)
            .Include(p => p.Author)
            .Include(p => p.Taxon)
            .ThenInclude(t => t.Parent)
            .ThenInclude(t => t.Parent)
            .ThenInclude(t => t.Parent)
            .ToListAsync();
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

    public async Task<List<Comment>> GetCommentsByAuthorEmailAsync(string authorEmail, int pageNumber, int pageSize)
    {
        return await _context.Posts
            .OfType<Comment>()
            .Where(c => c.Author.Email == authorEmail)
            .OrderByDescending(c => c.TimeStamp)
            .ThenByDescending(c => c.PostId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(c => c.Author)
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


    public async Task<List<Proposal>> GetProposalsByAuthorEmailAsync(string authorEmail, int pageNumber, int pageSize)
    {
        return await _context.Posts
            .OfType<Proposal>()
            .Where(c => c.Author.Email == authorEmail)
            .OrderByDescending(c => c.TimeStamp)
            .ThenByDescending(c => c.PostId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(c => c.Author)
            .Include(c => c.Taxon)
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

    public async Task<List<Taxon>> GetTaxaAsync()
    {
        return await _context.Taxa
            .OrderBy(t => t.TaxonId)
            .ToListAsync();
    }

    public async Task<Taxon?> GetTaxonByIdAsync(int taxonId)
    {
        return await _context.Taxa
            .FirstOrDefaultAsync(t => t.TaxonId == taxonId);
    }

    public async Task<Author?> GetAuthorByEmailAsync(string email)
    {
        return await _context.Authors
            .FirstOrDefaultAsync(a => a.Email == email);
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

    public async Task ForgetAuthorAsync(string authorEmail, bool keepObservations)
    {
        var author = await _context.Authors
            .FirstOrDefaultAsync(a => a.Email == authorEmail);

        if (author == null)
        {
            return;
        }

        var observations = await _context.Posts
            .OfType<Observation>()
            .Where(o => o.Author.Email == authorEmail)
            .ToListAsync();

        var comments = await _context.Posts
            .OfType<Comment>()
            .Where(c => c.Author.Email == authorEmail)
            .ToListAsync();

        var proposals = await _context.Posts
            .OfType<Proposal>()
            .Where(p => p.Author.Email == authorEmail)
            .ToListAsync();
        

        if (keepObservations)
        {
            var anonymousAuthor = await _context.Authors
                .FirstOrDefaultAsync(a => a.Email == DeletedUserEmail);

            if (anonymousAuthor == null)
            {
                anonymousAuthor = new Author
                {
                    Name = DeletedUserName,
                    Email = DeletedUserEmail
                };

                _context.Authors.Add(anonymousAuthor);
            }

            _context.Posts.RemoveRange(comments);
            _context.Posts.RemoveRange(proposals);
            foreach (var observation in observations)
            {
                observation.Author = anonymousAuthor;
            }
        }
        else
        {
            _context.Posts.RemoveRange(observations);
            _context.Posts.RemoveRange(comments);
            _context.Posts.RemoveRange(proposals);
        }

        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();
    }
}