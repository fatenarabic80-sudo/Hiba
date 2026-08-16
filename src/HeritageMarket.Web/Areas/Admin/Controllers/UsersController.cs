using HeritageMarket.Infrastructure.Identity;
using HeritageMarket.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeritageMarket.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentityRoles.Admin)]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // This page manages customer accounts only — administrators aren't listed or editable here.
    public async Task<IActionResult> Index()
    {
        var customers = await _userManager.GetUsersInRoleAsync(IdentityRoles.Customer);

        var result = new List<AdminUserListItemViewModel>();
        foreach (var user in customers.OrderBy(u => u.FullName))
        {
            result.Add(new AdminUserListItemViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Roles = await _userManager.GetRolesAsync(user),
                IsLockedOut = await _userManager.IsLockedOutAsync(user),
                CreatedAt = user.CreatedAt
            });
        }

        return View(result);
    }

    private async Task<bool> IsProtectedFromManagementAsync(ApplicationUser user)
        => await _userManager.IsInRoleAsync(user, IdentityRoles.Admin);

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleLock(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound();

        if (await IsProtectedFromManagementAsync(user))
        {
            TempData["ErrorMessage"] = "Administrator accounts can't be managed from here.";
            return RedirectToAction(nameof(Index));
        }

        var isLockedOut = await _userManager.IsLockedOutAsync(user);
        await _userManager.SetLockoutEndDateAsync(user, isLockedOut ? null : DateTimeOffset.MaxValue);

        TempData["StatusMessage"] = isLockedOut ? "User unlocked." : "User locked out.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SetRole(string id, string role)
    {
        if (role != IdentityRoles.Admin && role != IdentityRoles.Customer)
            return BadRequest();

        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound();

        if (await IsProtectedFromManagementAsync(user))
        {
            TempData["ErrorMessage"] = "Administrator accounts can't be managed from here.";
            return RedirectToAction(nameof(Index));
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, role);

        TempData["StatusMessage"] = $"{user.Email} is now a {role}.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound();
        if (await IsProtectedFromManagementAsync(user)) return Forbid();

        return View(new AdminUserEditViewModel
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Address = user.Address
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, AdminUserEditViewModel model)
    {
        if (id != model.Id) return BadRequest();

        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound();
        if (await IsProtectedFromManagementAsync(user)) return Forbid();

        if (!ModelState.IsValid) return View(model);

        user.FullName = model.FullName;
        user.Address = model.Address;

        if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
        {
            var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
            if (!setEmailResult.Succeeded)
            {
                foreach (var error in setEmailResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }
            await _userManager.SetUserNameAsync(user, model.Email);
        }

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }

        TempData["StatusMessage"] = "User details updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound();

        if (await IsProtectedFromManagementAsync(user))
        {
            TempData["ErrorMessage"] = "Administrator accounts can't be managed from here.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var result = await _userManager.DeleteAsync(user);
            TempData[result.Succeeded ? "StatusMessage" : "ErrorMessage"] =
                result.Succeeded ? "User deleted." : string.Join(' ', result.Errors.Select(e => e.Description));
        }
        catch (DbUpdateException)
        {
            TempData["ErrorMessage"] = "Could not delete this user — they have existing orders, which are kept for record-keeping. Lock the account instead if you want to block access.";
        }

        return RedirectToAction(nameof(Index));
    }
}
