using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bison.Infrastructure;
using Bison.Core;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Bison.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly IObservationService _service;
    public List<ObservationViewModel> Observations { get; set; } = new();

    public int CurrentPage {get; set; }

    public bool IsOwnTimeline { get; set; }

    [BindProperty]
    public string ObservationText { get; set; } = "";

    [BindProperty]
    public int NewObservationTaxonId { get; set; }

    public List<Taxon> Taxons { get; set; } = new();

    [BindProperty]
    public IFormFile? ImageFile { get; set; }

    public UserTimelineModel(IObservationService service)
    {
        _service = service;
    }

    public async Task<ActionResult> OnGet(string author, [FromQuery] int page)
    {
        if (page < 1)
        {
            page = 1;
        }

        CurrentPage = page;

        Taxons = await _service.GetTaxons();

        var authorEmail = User.FindFirstValue(ClaimTypes.Email);

        if (!string.IsNullOrEmpty(authorEmail))
        {
            IsOwnTimeline = await _service.IsAuthor(authorEmail, author);
        }

        Observations = await _service.GetObservationsFromAuthor(author, page);

        return Page();
    }

    public async Task<IActionResult> OnPostCreateObservationAsync(string author)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
            return Challenge();

        var authorEmail = User.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(authorEmail))
            return Forbid();

        if (!await _service.IsAuthor(authorEmail, author))
            return Forbid();

        bool success = await _service.CreateObservation(
            authorEmail,
            ObservationText,
            NewObservationTaxonId,
            ImageFile
        );

        if (!success)
        {
            IsOwnTimeline = true;
            Taxons = await _service.GetTaxons();
            Observations = await _service.GetObservationsFromAuthor(author, 1);
            CurrentPage = 1;
            return Page();
        }

        return RedirectToPage(new { author });
    }

    public async Task<IActionResult> OnPostUploadPictureAsync(string author, int id, int page)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
            return Challenge();

        if (ImageFile == null)
            return RedirectToPage(new { author, page });

        var authorEmail = User.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(authorEmail))
            return Forbid();

        bool success = await _service.AddPictureToObservationAsync(id, authorEmail, ImageFile);

        if (!success)
            return BadRequest("Could not upload picture.");

        return RedirectToPage(new { author, page });
    }
}
