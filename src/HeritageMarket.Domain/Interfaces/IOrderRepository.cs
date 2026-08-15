using HeritageMarket.Domain.Entities;

namespace HeritageMarket.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<IReadOnlyList<Order>> GetByUserIdAsync(string applicationUserId);
    Task<Order?> GetWithItemsAsync(int id);
}
