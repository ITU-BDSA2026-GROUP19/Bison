using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly IObservationService _service;
    public List<ObservationViewModel> Observations { get; set; }
    public int CurrentPage {get; set; }

    public PublicModel(IObservationService service)
    {
        _service = service;
    }

    public ActionResult OnGet([FromQuery] int page)
    {
        if (page < 1)
        {
            page = 1;
        }

        CurrentPage = page;
        Observations = _service.GetObservations(page);
        return Page();
    }
}
