using HeritageMarket.Infrastructure.Identity;
using HeritageMarket.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HeritageMarket.Web.Areas.Admin.Controllers;

[Area("Admin")]
[AllowAnonymous]
public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<AccountController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    public IActionResult Login(string? returnUrl = null)
    {
        if (User.IsInRole(IdentityRoles.Admin))
            return RedirectToLocalOrDashboard(returnUrl);

        return View(new AdminLoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AdminLoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is not null && await _userManager.IsInRoleAsync(user, IdentityRoles.Admin))
            {
                _logger.LogInformation("Admin {Email} logged in via the admin portal.", model.Email);
                return RedirectToLocalOrDashboard(model.ReturnUrl);
            }

            // Valid credentials, but not an administrator: this login is for admins only.
            await _signInManager.SignOutAsync();
            ModelState.AddModelError(string.Empty, "This login is for administrators only.");
            return View(model);
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "This account has been locked out due to multiple failed attempts. Try again later.");
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    private IActionResult RedirectToLocalOrDashboard(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
    }
}
