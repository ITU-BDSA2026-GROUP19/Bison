using Microsoft.AspNetCore.Mvc.RazorPages;
using Bison.Infrastructure;
using Bison.Core;
using System.Security.Claims;

namespace Bison.Razor.Pages;
public class AboutModel : PageModel
{
    private readonly IObservationService _service;
    public List<ObservationViewModel> Observations { get; set; } = new();
    public int CurrentPage {get; set; }
    

    public AboutModel(IObservationService service)
    {
        _service = service;
    }
}