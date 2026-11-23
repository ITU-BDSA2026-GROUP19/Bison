using Microsoft.AspNetCore.Mvc.RazorPages;
using Bison.Infrastructure;
using Bison.Core;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bison.Razor.Pages;

[Authorize]
public class AchievementsModel : PageModel
{
    private readonly IAchievementService _service;

    public List<AchievementViewModel> Achievements { get; set; } = new();

    public AchievementsModel(IAchievementService service)
    {
        _service = service;
    }

    public async Task<IActionResult> OnGet()
    {
        var authorEmail = User.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(authorEmail))
        {
            return Forbid();
        }

        Achievements = await _service.GetAchievementsForAuthorAsync(authorEmail);

        return Page();
    }
}