using HeritageMarket.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HeritageMarket.Infrastructure.Identity;

public class UserDirectoryService : IUserDirectoryService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserDirectoryService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<string> GetDisplayNameAsync(string applicationUserId)
    {
        var user = await _userManager.FindByIdAsync(applicationUserId);
        if (user is null) return "Unknown User";
        return !string.IsNullOrWhiteSpace(user.FullName) ? user.FullName : user.Email ?? "Unknown User";
    }

    public async Task<string> GetEmailAsync(string applicationUserId)
    {
        var user = await _userManager.FindByIdAsync(applicationUserId);
        return user?.Email ?? "Unknown";
    }

    public async Task<int> GetCustomerCountAsync()
    {
        var customers = await _userManager.GetUsersInRoleAsync(IdentityRoles.Customer);
        return customers.Count;
    }
}
