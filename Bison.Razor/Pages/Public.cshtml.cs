using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bison.Model;

namespace Bison.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly IObservationService _service;
    public List<ObservationViewModel> Observations { get; set; } = new();
    public List<Taxon> Taxons { get; set; } = new();
    public int CurrentPage {get; set; }
    public int? SelectedTaxonId { get; set; }

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
}
