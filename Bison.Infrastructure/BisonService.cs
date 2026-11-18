using Bison.Core;
using Microsoft.AspNetCore.Http;

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
    public Task<bool> CreateObservation(string authorEmail, string text, int taxonId);
    public Task<bool> IsAuthor(string authorEmail, string authorName);
    public Task<bool> AddPictureToObservationAsync(int observationId, string authorEmail, IFormFile imageFile);
}

public class ObservationService : IObservationService
{
    private const int PageSize = 32;
    private readonly IPostRepository _postRepository;
    private readonly ITaxonFilterService _taxonFilterService;
    private readonly IImageService _imageService;

    public ObservationService(IPostRepository db, ITaxonFilterService taxonFilterService, IImageService imageService)
    {
        _postRepository = db;
        _taxonFilterService = taxonFilterService;
        _imageService = imageService;
    }

    public async Task<List<ObservationViewModel>> GetObservations(int page)
    {
            List<Observation> observations = await _postRepository.GetPostsByPageAsync(page, PageSize);
            return observations.Select(o => new ObservationViewModel(
                o.PostId,
                o.Author.Name,
                o.Author.Email,
                o.Text,
                o.TimeStamp.ToString("MM/dd/yy H:mm:ss"),
                o.PicturePath
            )).ToList();
    }

    public async Task<List<ObservationViewModel>> GetObservationsFromAuthor(string author, int page)
    {
        List<Observation> observations = await _postRepository.GetPostsByAuthorAsync(author, page, PageSize);
        return observations.Select(o => new ObservationViewModel(
            o.PostId,
            o.Author.Name,
            o.Author.Email,
            o.Text,
            o.TimeStamp.ToString("MM/dd/yy H:mm:ss"),
            o.PicturePath
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
                observation.PicturePath
            )
        };
    }

    public async Task<List<ObservationViewModel>> GetComments(int observationId)
    {
        var comments = await _postRepository.GetCommentsByObservationIdAsync(observationId);
        return comments.Select(c => new ObservationViewModel(
            c.PostId,
            c.Author.Name,
            c.Author.Email,
            c.Text,
            c.TimeStamp.ToString("MM/dd/yy H:mm:ss"),
            null
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
            o.PostId,
            o.Author.Name,
            o.Author.Email,
            o.Text,
            o.TimeStamp.ToString("MM/dd/yy H:mm:ss"),
            o.PicturePath
        )).ToList();
    }

    public async Task<bool> CreateObservation(string authorEmail, string text, int taxonId)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var author = await _postRepository.GetAuthorByEmailAsync(authorEmail);
        var taxon = await _postRepository.GetTaxonByIdAsync(taxonId);

        if (author == null || taxon == null)
        {
            return false;
        }

        var observation = new Observation
        {
            Text = text,
            TimeStamp = DateTime.UtcNow,
            Author = author,
            Taxon = taxon
        };

        await _postRepository.AddPostAsync(observation);

        return true;
    }

    public async Task<bool> IsAuthor(string authorEmail, string authorName)
    {
        var author = await _postRepository.GetAuthorByEmailAsync(authorEmail);
        return author != null && author.Name == authorName;
    }

    public async Task<bool> AddPictureToObservationAsync(int observationId, string authorEmail, IFormFile imageFile)
    {
        var observation = await _postRepository.GetObservationByIdAsync(observationId);
        if (observation == null || observation.Author.Email != authorEmail)
        {
            return false;
        }
        if (!await _imageService.ValidateImageAsync(imageFile))
        {
            return false;
        }
        observation.PicturePath = await _imageService.SaveImageAsync(imageFile);
        await _postRepository.UpdatePostAsync(observation);
        return true;
    }
}