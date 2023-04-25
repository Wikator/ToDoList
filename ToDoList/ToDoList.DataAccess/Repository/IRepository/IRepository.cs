using System.Linq.Expressions;

namespace ToDoList.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Get all entities from the database
        /// </summary>
        /// <param name="filter">Any filter to not include specific elements</param>
        /// <param name="includeProperties">Properties to include, separated with commas</param>
        /// <returns>An IEnumerable of entities</returns>

        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);

        /// <summary>
        /// Get all entities from the database with ordering
        /// </summary>
        /// <typeparam name="DataType">A datatype of field that will be used to order elements</typeparam>
        /// <param name="order">How to order the elements</param>
        /// <param name="filter">Any filter to not include specific elements</param>
        /// <param name="includeProperties">Properties to include, separated with commas</param>
        /// <returns>An IEnumerable of entities</returns>

        Task<IEnumerable<T>> GetAllAsync<DataType>(Expression<Func<T, DataType>> order, Expression<Func<T, bool>>? filter = null, string? includeProperties = null);

        /// <summary>
        /// Returns the first entity that matches the filter
        /// </summary>
        /// <param name="filter">Filter used to find the specific element</param>
        /// <param name="includeProperties">Properties to include, separated with commas</param>
        /// <returns>An entity from database</returns>

        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, string? includeProperties = null);

        /// <summary>
        /// Adds an entity to the database
        /// </summary>
        /// <param name="entity">Entity to add</param>

        void Add(T entity);

        /// <summary>
        /// Updates an entity in the database
        /// </summary>
        /// <param name="entity">Entity to update</param>

        void Update(T entity);

        /// <summary>
        /// Removes an entity from the database
        /// </summary>
        /// <param name="entity">Entity to remove</param>

        void Remove(T entity);

        /// <summary>
        /// Removes a range of entities from the database
        /// </summary>
        /// <param name="entities">Entities to remove</param>

        void RemoveRange(IEnumerable<T> entities);
    }
}
