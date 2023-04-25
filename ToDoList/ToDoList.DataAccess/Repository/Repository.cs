using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ToDoList.DataAccess.Data;
using ToDoList.DataAccess.Repository.IRepository;

namespace ToDoList.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            dbSet = db.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter is not null)
            {
                query = query.Where(filter);
            }

            if (includeProperties is not null)
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync<DataType>(Expression<Func<T, DataType>> order, Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter is not null)
            {
                query = query.Where(filter);
            }

            if (includeProperties is not null)
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.OrderBy(order).ToListAsync();
        }

        public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            query = query.Where(filter);

            if (includeProperties is not null)
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            dbSet.Update(entity);
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entitities)
        {
            dbSet.RemoveRange(entitities);
        }
    }
}
