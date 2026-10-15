public record ObservationViewModel(string Author, string Message, string Timestamp);
public record ObservationDetailsViewModel(int Id, string Author, string Message, string Timestamp);
public record ProposalViewModel(string Author, int TaxonId, string Timestamp);

public interface IObservationService
{
    public Task<List<ObservationViewModel>> GetObservations(int page);
    public Task<List<ObservationViewModel>> GetObservationsFromAuthor(string author, int page);

    public Task<List<ObservationDetailsViewModel>> GetObservationById(int id);
    public Task<List<ObservationViewModel>> GetComments(int observationId);
    public Task<List<ProposalViewModel>> GetProposals(int observationId);
}

public class ObservationService : IObservationService
{
    private const int PageSize = 32;
    private readonly IPostRepository _postRepository;

    public ObservationService(IPostRepository db)
    {
        _postRepository = db;
    }

    public async Task<List<ObservationViewModel>> GetObservations(int page)
    {
            List<Observation> observations = await _postRepository.GetPostsByPageAsync(page, PageSize);
            return observations.Select(o => new ObservationViewModel(
                o.Author.Name,
                o.Text,
                o.TimeStamp.ToString("MM/dd/yy H:mm:ss")
            )).ToList();
    }

    public async Task<List<ObservationViewModel>> GetObservationsFromAuthor(string author, int page)
    {
        List<Observation> observations = await _postRepository.GetPostsByAuthorAsync(author, page, PageSize);
        return observations.Select(o => new ObservationViewModel(
            o.Author.Name,
            o.Text,
            o.TimeStamp.ToString("MM/dd/yy H:mm:ss")
        )).ToList();
    }

    public async Task<List<ObservationDetailsViewModel>> GetObservationById(int id)
    {
        var observation = await _postRepository.GetObservationByIdAsync(id);
        if (observation == null)
        {
            return new List<ObservationDetailsViewModel>();
        }

        return new List<ObservationDetailsViewModel>
        {
            new ObservationDetailsViewModel(
                observation.PostId,
                observation.Author.Name,
                observation.Text,
                observation.TimeStamp.ToString("MM/dd/yy H:mm:ss")
            )
        };
    }

    public async Task<List<ObservationViewModel>> GetComments(int observationId)
    {
        var comments = await _postRepository.GetCommentsByObservationIdAsync(observationId);
        return comments.Select(c => new ObservationViewModel(
            c.Author.Name,
            c.Text,
            c.TimeStamp.ToString("MM/dd/yy H:mm:ss")
        )).ToList();
    }

    public async Task<List<ProposalViewModel>> GetProposals(int observationId)
    {
        var proposals = await _postRepository.GetProposalsByObservationIdAsync(observationId);
        return proposals.Select(p => new ProposalViewModel(
            p.Author.Name,
            p.Taxon.TaxonId,
            p.TimeStamp.ToString("MM/dd/yy H:mm:ss")
        )).ToList();
    }
}