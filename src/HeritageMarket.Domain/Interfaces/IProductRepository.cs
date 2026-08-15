using HeritageMarket.Domain.Entities;

namespace HeritageMarket.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetWithDetailsAsync(int id);
}
