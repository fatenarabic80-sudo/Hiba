using HeritageMarket.Infrastructure.Identity;
using HeritageMarket.Web.Helpers;
using HeritageMarket.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HeritageMarket.Web.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IWebHostEnvironment _environment;

    public IndexModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IWebHostEnvironment environment)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _environment = environment;
    }

    [BindProperty]
    public ProfileViewModel ProfileInput { get; set; } = new();

    [BindProperty]
    public ChangePasswordViewModel PasswordInput { get; set; } = new();

    public bool HasPassword { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        ProfileInput = new ProfileViewModel
        {
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Address = user.Address,
            ExistingProfileImageUrl = user.ProfileImageUrl
        };
        HasPassword = await _userManager.HasPasswordAsync(user);

        return Page();
    }

    public async Task<IActionResult> OnPostProfileAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        ModelState.Clear();
        if (!TryValidateModel(ProfileInput, nameof(ProfileInput)))
        {
            HasPassword = await _userManager.HasPasswordAsync(user);
            return Page();
        }

        user.FullName = ProfileInput.FullName;
        user.Address = ProfileInput.Address;

        if (ProfileInput.ProfileImageFile is not null)
        {
            try
            {
                var imageUrl = await FileUploadHelper.SaveImageAsync(ProfileInput.ProfileImageFile, _environment, "profiles");
                if (imageUrl is not null)
                    user.ProfileImageUrl = imageUrl;
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                HasPassword = await _userManager.HasPasswordAsync(user);
                return Page();
            }
        }

        await _userManager.UpdateAsync(user);
        await _signInManager.RefreshSignInAsync(user);

        StatusMessage = "Your profile has been updated.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostPasswordAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        ModelState.Clear();
        if (!TryValidateModel(PasswordInput, nameof(PasswordInput)))
        {
            var profile = await _userManager.GetUserAsync(User);
            ProfileInput = new ProfileViewModel { FullName = profile!.FullName, Email = profile.Email ?? string.Empty, Address = profile.Address, ExistingProfileImageUrl = profile.ProfileImageUrl };
            HasPassword = await _userManager.HasPasswordAsync(user);
            return Page();
        }

        var result = await _userManager.ChangePasswordAsync(user, PasswordInput.OldPassword, PasswordInput.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            var profile = await _userManager.GetUserAsync(User);
            ProfileInput = new ProfileViewModel { FullName = profile!.FullName, Email = profile.Email ?? string.Empty, Address = profile.Address, ExistingProfileImageUrl = profile.ProfileImageUrl };
            HasPassword = true;
            return Page();
        }

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "Your password has been changed.";
        return RedirectToPage();
    }
}
