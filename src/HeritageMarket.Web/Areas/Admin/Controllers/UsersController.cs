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

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.OrderBy(u => u.FullName).ToListAsync();
        var currentUserId = _userManager.GetUserId(User);

        var result = new List<AdminUserListItemViewModel>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new AdminUserListItemViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Roles = roles,
                IsLockedOut = await _userManager.IsLockedOutAsync(user),
                CreatedAt = user.CreatedAt
            });
        }

        ViewData["CurrentUserId"] = currentUserId;
        return View(result);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleLock(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound();

        if (id == _userManager.GetUserId(User))
        {
            TempData["ErrorMessage"] = "You cannot lock your own account.";
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

        if (id == _userManager.GetUserId(User))
        {
            TempData["ErrorMessage"] = "You cannot change your own role.";
            return RedirectToAction(nameof(Index));
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, role);

        TempData["StatusMessage"] = $"{user.Email} is now a {role}.";
        return RedirectToAction(nameof(Index));
    }
}
