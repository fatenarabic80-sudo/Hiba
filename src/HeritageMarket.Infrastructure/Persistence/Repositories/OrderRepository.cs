using HeritageMarket.Domain.Entities;
using HeritageMarket.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeritageMarket.Infrastructure.Persistence.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Order>> GetByUserIdAsync(string applicationUserId) =>
        await DbSet
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Where(o => o.ApplicationUserId == applicationUserId)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Order?> GetWithItemsAsync(int id) =>
        await DbSet
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
}
