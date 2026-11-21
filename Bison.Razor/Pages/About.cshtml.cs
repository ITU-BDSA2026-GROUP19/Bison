using Microsoft.AspNetCore.Mvc.RazorPages;
using Bison.Infrastructure;
using Bison.Core;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bison.Razor.Pages;

[Authorize]
public class AboutModel : PageModel
{
    private readonly IObservationService _service;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly BisonDBContext _context;
    public List<ObservationViewModel> Observations { get; set; } = new();
    public List<ObservationViewModel> Comments { get; set; } = new();
    public List<ProposalViewModel> Proposals { get; set; } = new();
    public int CurrentPage {get; set; }
    

    public AboutModel(IObservationService service, 
        UserManager<IdentityUser> userManager, 
        SignInManager<IdentityUser> signInManager,
        BisonDBContext context)
    {
        _service = service;
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
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

    public async Task<IActionResult> OnPostForgetMeAsync(bool keepObservations)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Forbid();
        }

        if (string.IsNullOrEmpty(user.Email))
        {
            return Forbid();
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        
        await _service.ForgetAuthorAsync(user.Email, keepObservations);

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            await transaction.RollbackAsync();
            return Forbid();
        }
        await transaction.CommitAsync();

        await _signInManager.SignOutAsync();

        return RedirectToPage("/Public");
    }
}