using Bison.Model;

public interface IPostRepository
{
    Task<List<Observation>> GetPostsByPageAsync(int pageNumber, int pageSize);
    Task<Observation?> GetObservationByIdAsync(int postId);
    Task AddPostAsync(Post post);
    Task UpdatePostAsync(Post post);
    Task DeletePostAsync(int postId);
    Task<List<Observation>> GetPostsByAuthorAsync(string authorName, int pageNumber, int pageSize);
    Task<List<Comment>> GetCommentsByObservationIdAsync(int observationId);
    Task<List<Proposal>> GetProposalsByObservationIdAsync(int observationId);
}