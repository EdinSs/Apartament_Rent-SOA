using EasyRent.Application.Interfaces.Repositories;
using EasyRent.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EasyRent.Infrastructure.Repositories;

/// <summary>
/// Shared EF Core CRUD implementation for entities with an integer key.
/// Entity-specific repositories inherit this and add their own queries.
/// Methods are <c>virtual</c> so a derived repo can override them (e.g. to eager-load).
/// </summary>
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> Set;

    public GenericRepository(AppDbContext context)
    {
        Context = context;
        Set = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id) => await Set.FindAsync(id);

    public virtual async Task<IEnumerable<T>> GetAllAsync() => await Set.ToListAsync();

    public virtual async Task AddAsync(T entity) => await Set.AddAsync(entity);

    public virtual void Update(T entity) => Set.Update(entity);

    public virtual void Delete(T entity) => Set.Remove(entity);

    /// <summary>Commits all pending changes in one transaction; returns affected row count.</summary>
    public async Task<int> SaveChangesAsync() => await Context.SaveChangesAsync();
}
