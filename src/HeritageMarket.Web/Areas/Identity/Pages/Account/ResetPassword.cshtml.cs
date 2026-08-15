using System.Text;
using HeritageMarket.Infrastructure.Identity;
using HeritageMarket.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace HeritageMarket.Web.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class ResetPasswordModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ResetPasswordModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [BindProperty]
    public ResetPasswordViewModel Input { get; set; } = new();

    public IActionResult OnGet(string? code = null, string? email = null)
    {
        if (code is null || email is null)
            return BadRequest("A code and email must be supplied for password reset.");

        Input = new ResetPasswordViewModel
        {
            Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code)),
            Email = email
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = await _userManager.FindByEmailAsync(Input.Email);
        if (user is null)
            return RedirectToPage("./ResetPasswordConfirmation");

        var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
        if (result.Succeeded)
            return RedirectToPage("./ResetPasswordConfirmation");

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return Page();
    }
}
