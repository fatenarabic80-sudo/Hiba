using HeritageMarket.Domain.Entities;
using HeritageMarket.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeritageMarket.Infrastructure.Persistence.Repositories;

public class CartRepository : Repository<Cart>, ICartRepository
{
    public CartRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Cart?> GetByUserIdAsync(string applicationUserId) =>
        await DbSet
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.ApplicationUserId == applicationUserId);
}
