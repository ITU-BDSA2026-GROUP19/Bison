using Microsoft.AspNetCore.Mvc.RazorPages;
using Bison.Infrastructure;
using Bison.Core;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Bison.Razor.Pages;

[Authorize]
public class AboutModel : PageModel
{
    private readonly IObservationService _service;
    public List<ObservationViewModel> Observations { get; set; } = new();
    public List<ObservationViewModel> Comments { get; set; } = new();
    public List<ProposalViewModel> Proposals { get; set; } = new();
    public int CurrentPage {get; set; }
    

    public AboutModel(IObservationService service)
    {
        _service = service;
    }

    public async Task<IActionResult> OnGet([FromQuery] int page)
    {
        if (page < 1)
        {
            page = 1;
        }

        CurrentPage = page;

        var authorEmail = User.FindFirstValue(ClaimTypes.Email);
        var author = User.FindFirstValue(ClaimTypes.Name);

        if (!string.IsNullOrEmpty(authorEmail) && !string.IsNullOrEmpty(author))
        {
            Observations = await _service.GetObservationsFromAuthor(author, page);
            Comments = await _service.GetCommentsFromAuthor(authorEmail, page);
            Proposals = await _service.GetProposalsFromAuthor(authorEmail, page);
        }
        else
        {
            return Forbid();
        }

        return Page();
    }
}