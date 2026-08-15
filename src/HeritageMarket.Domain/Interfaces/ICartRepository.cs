using HeritageMarket.Domain.Entities;

namespace HeritageMarket.Domain.Interfaces;

public interface ICartRepository : IRepository<Cart>
{
    Task<Cart?> GetByUserIdAsync(string applicationUserId);
}
