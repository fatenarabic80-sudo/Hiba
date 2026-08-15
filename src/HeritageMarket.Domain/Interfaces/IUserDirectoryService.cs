namespace HeritageMarket.Domain.Interfaces;

public interface IUserDirectoryService
{
    Task<string> GetDisplayNameAsync(string applicationUserId);
    Task<int> GetCustomerCountAsync();
}
