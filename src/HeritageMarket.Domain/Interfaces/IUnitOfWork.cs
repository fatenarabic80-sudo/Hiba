using HeritageMarket.Domain.Entities;

namespace HeritageMarket.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    IRepository<Category> Categories { get; }
    IRepository<Country> Countries { get; }
    ICartRepository Carts { get; }
    IOrderRepository Orders { get; }
    IRepository<Review> Reviews { get; }
    IRepository<Notification> Notifications { get; }
    IRepository<WishlistItem> WishlistItems { get; }

    Task<int> SaveChangesAsync();
}
