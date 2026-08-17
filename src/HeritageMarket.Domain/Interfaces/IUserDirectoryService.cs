namespace HeritageMarket.Domain.Interfaces;

public interface IUserDirectoryService
{
    Task<string> GetDisplayNameAsync(string applicationUserId);
    Task<string> GetEmailAsync(string applicationUserId);
    Task<int> GetCustomerCountAsync();
}
