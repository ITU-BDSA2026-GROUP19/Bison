using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly IObservationService _service;
    public List<ObservationViewModel> Observations { get; set; } = new();

    public int CurrentPage {get; set; }

    public UserTimelineModel(IObservationService service)
    {
        _service = service;
    }

    public ActionResult OnGet(string author, [FromQuery] int page)
    {
        if (page < 1)
        {
            page = 1;
        }

        CurrentPage = page;
        Observations = _service.GetObservationsFromAuthor(author, page);
        return Page();
    }
}
