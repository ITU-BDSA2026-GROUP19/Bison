using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bison.Core;
using Bison.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Bison.Razor.Pages;

public class ObservationModel : PageModel
{
    private readonly IObservationService _service;

    public ObservationDetailsViewModel Observation { get; set; } = new ObservationDetailsViewModel(0, "", "", "", null, null);
    public List<ObservationViewModel> Comments { get; set; } = new List<ObservationViewModel>();
    public List<ProposalViewModel> Proposals { get; set; } = new List<ProposalViewModel>();
    public List<Taxon> Taxons { get; set; } = new List<Taxon>();

    [BindProperty]
    public int SelectedTaxonId { get; set; }

    [BindProperty]
    public IFormFile? ImageFile { get; set; }

    [BindProperty]
    public string CommentText { get; set; } = "";

    [BindProperty]
    public string? PictureLicense { get; set; }

    public ObservationModel(IObservationService service)
    {
        _service = service;
    }

    public async Task<ActionResult> OnGet(int id)
    {
        if (id == 0)
        {
            return Redirect("/");
        }

        List<ObservationDetailsViewModel> found = await _service.GetObservationById(id);

        if (found.Count == 0)
        {
            return NotFound();
        }

        Observation = found[0];
        Taxons = await _service.GetTaxons();
        Comments = await _service.GetComments(id);
        Proposals = await _service.GetProposals(id);

        return Page();
    }

    public async Task<IActionResult> OnPostCreateCommentAsync(int id)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Challenge();
        }

        var authorEmail = User.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(authorEmail))
        {
            return Forbid();
        }

        bool success = await _service.CreateComment(
            authorEmail,
            id,
            CommentText
        );

        if (!success)
        {
            return BadRequest();
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostCreateProposalAsync(int id)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Challenge();
        }

        var authorEmail = User.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(authorEmail))
        {
            return Forbid();
        }

        bool success = await _service.CreateProposal(
            authorEmail,
            id,
            SelectedTaxonId
        );

        if (!success)
        {
            return BadRequest();
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostUploadPictureAsync(int id)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Challenge();
        }

        if (ImageFile == null)
        {
            return RedirectToPage(new { id });
        }

        if (string.IsNullOrEmpty(PictureLicense))
        {
            ModelState.AddModelError("PictureLicense", "Picture license is required.");
            return Page();
        }

        var authorEmail = User.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(authorEmail))
        {
            return Forbid();
        }

        bool success = await _service.AddPictureToObservationAsync(
            id,
            authorEmail,
            ImageFile,
            PictureLicense
        );

        if (!success)
        {
            return Forbid();
        }

        return RedirectToPage(new { id });
    }
}