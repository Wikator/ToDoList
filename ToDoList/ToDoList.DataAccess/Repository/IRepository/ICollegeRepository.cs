using System.Linq.Expressions;
using ToDoList.Models;

namespace ToDoList.DataAccess.Repository.IRepository
{
    public interface ICollegeRepository : IRepository<College>
    {
        /// <summary>
        /// A method that returns a list of colleges created by admins
        /// </summary>
        /// <param name="ignoreNoneGroups">Whether to ignore or not groups that are considered as "None". Elements that belong to ApplicationUser will not be ignored</param>
        /// <param name="applicationUser">Providing an ApplicationUser will also include elements created by this user</param>
        /// <param name="filter">Any additional filters</param>
        /// <returns>An IEnumerable of colleges</returns>

        Task<IEnumerable<College>> GetCollegesAsync(bool ignoreNoneGroups, ApplicationUser? applicationUser = null, Expression<Func<College, bool>>? filter = null);


        /// <summary>
        /// A method that returns a list of colleges created by admins with ordering
        /// </summary>
        /// <typeparam name="DataType">A datatype of field that will be used to order elements</typeparam>
        /// <param name="ignoreNoneGroups">Whether to ignore or not groups that are considered as "None". Elements that belong to ApplicationUser will not be ignored</param>
        /// <param name="order">How to order the elements</param>
        /// <param name="applicationUser">Providing an ApplicationUser will also include elements created by this user</param>
        /// <param name="filter">Any additional filters</param>
        /// <returns>An IEnumerable of colleges</returns>

        Task<IEnumerable<College>> GetCollegesAsync<DataType>(bool ignoreNoneGroups, Expression<Func<College, DataType>> order, ApplicationUser? applicationUser = null, Expression<Func<College, bool>>? filter = null);

        void RemoveCollege(College college, string wwwwRootPath);
    }
}
