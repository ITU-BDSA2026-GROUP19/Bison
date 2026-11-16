using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bison.Core;
using Bison.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Bison.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly IObservationService _service;
    public List<ObservationViewModel> Observations { get; set; } = new();
    public List<Taxon> Taxons { get; set; } = new();
    public int CurrentPage {get; set; }
    public int? SelectedTaxonId { get; set; }

    [BindProperty]
    public IFormFile? ImageFile { get; set; }

    public PublicModel(IObservationService service)
    {
        _service = service;
    }

    public async Task<IActionResult> OnGet([FromQuery] int page, int? taxonId)
    {
        if (page < 1)
        {
            page = 1;
        }
        
        Taxons = await _service.GetTaxons();

        CurrentPage = page;

        if (taxonId != null)
        {
            SelectedTaxonId = taxonId;
            Observations = await _service.GetObservationsByTaxon(taxonId.Value, page);
        }
        else
        {
            Observations = await _service.GetObservations(page);
        }
        return Page();
    }

    public async Task<IActionResult> OnPostUploadPictureAsync(int id, int page, int? taxonId)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
            return Challenge();

        if (ImageFile == null)
            return RedirectToPage(new { page, taxonId });

        var authorEmail = User.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(authorEmail))
            return Forbid();

        bool success = await _service.AddPictureToObservationAsync(id, authorEmail, ImageFile);

        if (!success)
            return BadRequest("Could not upload picture.");

        return RedirectToPage(new { page, taxonId });
    }
}
