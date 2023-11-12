#region

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ToDoList.DataAccess.Repository.IRepository;

#endregion

namespace ToDoList.DataAccess.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly DbSet<T> DbSet;

    public Repository(DbContext db)
    {
        DbSet = db.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null,
        string? includeProperties = null)
    {
        var query = DbSet.AsSplitQuery();

        if (filter is not null) query = query.Where(filter);

        if (includeProperties is null) return await query.ToListAsync();
        
        query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync<TDataType>(Expression<Func<T, TDataType>> order,
        Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
    {
        var query = DbSet.AsSplitQuery();

        if (filter is not null) query = query.Where(filter);

        if (includeProperties is null) return await query.OrderBy(order).ToListAsync();
        
        query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

        return await query.OrderBy(order).ToListAsync();
    }

    public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, string? includeProperties = null)
    {
        IQueryable<T> query = DbSet;

        query = query.Where(filter);

        if (includeProperties is null) return await query.FirstOrDefaultAsync();
        
        query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

        return await query.FirstOrDefaultAsync();
    }

    public void Add(T entity)
    {
        DbSet.Add(entity);
    }

    public void Update(T entity)
    {
        DbSet.Update(entity);
    }

    public void Remove(T entity)
    {
        DbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        DbSet.RemoveRange(entities);
    }
}