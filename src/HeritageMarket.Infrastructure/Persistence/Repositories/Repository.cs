using System.Linq.Expressions;
using HeritageMarket.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeritageMarket.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext Context;
    protected readonly DbSet<T> DbSet;

    public Repository(ApplicationDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id) => await DbSet.FindAsync(id);

    public virtual async Task<IReadOnlyList<T>> GetAllAsync() => await DbSet.ToListAsync();

    public IQueryable<T> Query() => DbSet.AsQueryable();

    public virtual async Task AddAsync(T entity) => await DbSet.AddAsync(entity);

    public virtual void Update(T entity) => DbSet.Update(entity);

    public virtual void Remove(T entity) => DbSet.Remove(entity);

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate) => await DbSet.AnyAsync(predicate);
}
