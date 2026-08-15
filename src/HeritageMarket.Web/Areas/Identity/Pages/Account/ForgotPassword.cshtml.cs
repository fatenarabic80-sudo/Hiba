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
public class ForgotPasswordModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ForgotPasswordModel> _logger;

    public ForgotPasswordModel(UserManager<ApplicationUser> userManager, ILogger<ForgotPasswordModel> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    [BindProperty]
    public ForgotPasswordViewModel Input { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = await _userManager.FindByEmailAsync(Input.Email);
        if (user is null || !await _userManager.IsEmailConfirmedAsync(user))
        {
            // Do not reveal that the user does not exist or is not confirmed.
            return RedirectToPage("./ForgotPasswordConfirmation");
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var resetUrl = Url.Page(
            "/Account/ResetPassword",
            pageHandler: null,
            values: new { area = "Identity", code, email = user.Email },
            protocol: Request.Scheme);

        _logger.LogInformation("Password reset link for {Email}: {ResetUrl}", user.Email, resetUrl);

        // No email sender is configured for this demo project; the reset link is surfaced
        // on the confirmation page so the reset flow can still be exercised end-to-end.
        TempData["ResetLink"] = resetUrl;
        return RedirectToPage("./ForgotPasswordConfirmation");
    }
}
