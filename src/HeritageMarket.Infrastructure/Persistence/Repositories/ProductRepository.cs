using HeritageMarket.Domain.Entities;
using HeritageMarket.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeritageMarket.Infrastructure.Persistence.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Product?> GetWithDetailsAsync(int id) =>
        await DbSet
            .Include(p => p.Category)
            .Include(p => p.Country)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Id == id);
}
