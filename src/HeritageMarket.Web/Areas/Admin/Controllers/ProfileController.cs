using HeritageMarket.Infrastructure.Identity;
using HeritageMarket.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HeritageMarket.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentityRoles.Admin)]
public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public ProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        return View(new AdminProfileViewModel
        {
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Address = user.Address
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(AdminProfileViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        model.Email = user.Email ?? string.Empty;
        if (!ModelState.IsValid) return View(model);

        user.FullName = model.FullName;
        user.Address = model.Address;
        await _userManager.UpdateAsync(user);

        TempData["StatusMessage"] = "Your profile has been updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(AdminChangePasswordViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Please correct the password form errors.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = string.Join(' ', result.Errors.Select(e => e.Description));
            return RedirectToAction(nameof(Index));
        }

        await _signInManager.RefreshSignInAsync(user);
        TempData["StatusMessage"] = "Your password has been changed.";
        return RedirectToAction(nameof(Index));
    }
}
