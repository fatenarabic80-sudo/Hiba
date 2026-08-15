using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HeritageMarket.Web.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class ForgotPasswordConfirmationModel : PageModel
{
    public string? ResetLink { get; set; }

    public void OnGet()
    {
        ResetLink = TempData["ResetLink"] as string;
    }
}
