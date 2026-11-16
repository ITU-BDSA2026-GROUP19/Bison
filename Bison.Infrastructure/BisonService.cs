using Bison.Core;

namespace Bison.Infrastructure;

public interface IObservationService
{
    public Task<List<ObservationViewModel>> GetObservations(int page);
    public Task<List<ObservationViewModel>> GetObservationsFromAuthor(string author, int page);

    public Task<List<ObservationDetailsViewModel>> GetObservationById(int id);
    public Task<List<ObservationViewModel>> GetComments(int observationId);
    public Task<List<ProposalViewModel>> GetProposals(int observationId);
    public Task<List<ObservationViewModel>> GetObservationsByTaxon(int taxonId, int page);
    public Task<List<Taxon>> GetTaxons();
    public Task<bool> CreateObservation(string authorName, string text, string pictureFileName);
    public Task<bool> CreateComment(string authorName, int observationId, string text);
    public Task<bool> CreateProposal(string authorName, int observationId, int taxonId);
}

public class ObservationService : IObservationService
{
    private const int PageSize = 32;
    private readonly IPostRepository _postRepository;
    private readonly ITaxonFilterService _taxonFilterService;

    public ObservationService(IPostRepository db, ITaxonFilterService taxonFilterService)
    {
        _postRepository = db;
        _taxonFilterService = taxonFilterService;
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
                observation.TimeStamp.ToString("MM/dd/yy H:mm:ss"),
                observation.PictureFileName
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

    public Task<List<Taxon>> GetTaxons()
    {
        return _postRepository.GetTaxaAsync();
    }

    public async Task<List<ObservationViewModel>> GetObservationsByTaxon(int taxonId, int page)
    {
        var taxon = await _postRepository.GetTaxonByIdAsync(taxonId);
        if (taxon == null)
        {
            return new List<ObservationViewModel>();
        }

        var observations = await _postRepository.GetAllObservationsAsync();
        var filteredObservations = _taxonFilterService.Filter(taxon, observations);

        return filteredObservations.Skip((page - 1) * PageSize)
            .Take(PageSize).Select(o => new ObservationViewModel(
                o.Author.Name,
                o.Text,
                o.TimeStamp.ToString("MM/dd/yy H:mm:ss")
            )).ToList();
    }

    public async Task<bool> CreateObservation(string authorName, string text, string pictureFileName)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        Author? author = await _postRepository.GetAuthorByNameAsync(authorName);

        if (author == null)
        {
            return false;
        }

        Observation observation = new Observation
        {
            Text = text.Trim(),
            TimeStamp = DateTime.Now,
            Author = author,
            PictureFileName = pictureFileName
        };

        await _postRepository.AddPostAsync(observation);
        return true;
    }

    public async Task<bool> CreateComment(string authorName, int observationId, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        Author? author = await _postRepository.GetAuthorByNameAsync(authorName);
        Observation? observation = await _postRepository.GetObservationByIdAsync(observationId);

        if (author == null || observation == null)
        {
            return false;
        }

        Comment comment = new Comment
        {
            Text = text.Trim(),
            TimeStamp = DateTime.Now,
            Author = author,
            Observation = observation
        };

        await _postRepository.AddPostAsync(comment);
        return true;
    }

    public async Task<bool> CreateProposal(string authorName, int observationId, int taxonId)
    {
        Author? author = await _postRepository.GetAuthorByNameAsync(authorName);
        Observation? observation = await _postRepository.GetObservationByIdAsync(observationId);
        Taxon? taxon = await _postRepository.GetTaxonByIdAsync(taxonId);

        if (author == null || observation == null || taxon == null)
        {
            return false;
        }

        Proposal proposal = new Proposal
        {
            Text = "",
            TimeStamp = DateTime.Now,
            Author = author,
            Observation = observation,
            Taxon = taxon
        };

        await _postRepository.AddPostAsync(proposal);
        return true;
    }
}