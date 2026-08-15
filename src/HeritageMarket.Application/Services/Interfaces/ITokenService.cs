namespace HeritageMarket.Application.Services.Interfaces;

public interface ITokenService
{
    string GenerateJwtToken(string userId, string email, IList<string> roles);
}
