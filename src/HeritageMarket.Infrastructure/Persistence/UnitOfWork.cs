using HeritageMarket.Domain.Entities;
using HeritageMarket.Domain.Interfaces;
using HeritageMarket.Infrastructure.Persistence.Repositories;

namespace HeritageMarket.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    private IProductRepository? _products;
    private IRepository<Category>? _categories;
    private IRepository<Country>? _countries;
    private ICartRepository? _carts;
    private IOrderRepository? _orders;
    private IRepository<Review>? _reviews;
    private IRepository<Notification>? _notifications;
    private IRepository<WishlistItem>? _wishlistItems;
    private IRepository<BookAccessRequest>? _bookAccessRequests;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IProductRepository Products => _products ??= new ProductRepository(_context);
    public IRepository<Category> Categories => _categories ??= new Repository<Category>(_context);
    public IRepository<Country> Countries => _countries ??= new Repository<Country>(_context);
    public ICartRepository Carts => _carts ??= new CartRepository(_context);
    public IOrderRepository Orders => _orders ??= new OrderRepository(_context);
    public IRepository<Review> Reviews => _reviews ??= new Repository<Review>(_context);
    public IRepository<Notification> Notifications => _notifications ??= new Repository<Notification>(_context);
    public IRepository<WishlistItem> WishlistItems => _wishlistItems ??= new Repository<WishlistItem>(_context);
    public IRepository<BookAccessRequest> BookAccessRequests => _bookAccessRequests ??= new Repository<BookAccessRequest>(_context);

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
