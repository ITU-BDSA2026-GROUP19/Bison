using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class ObservationModel : PageModel
{
    private readonly IObservationService _service;

    public ObservationDetailsViewModel Observation { get; set; } = new ObservationDetailsViewModel(0, "", "", "");
    public List<ObservationViewModel> Comments { get; set; } = new List<ObservationViewModel>();
    public List<ProposalViewModel> Proposals { get; set; } = new List<ProposalViewModel>();

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
        Comments = await _service.GetComments(id);
        Proposals = await _service.GetProposals(id);

        return Page();
    }
}