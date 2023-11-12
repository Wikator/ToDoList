#region

using ToDoList.Models;

#endregion

namespace ToDoList.DataAccess.Repository.IRepository;

public interface ISubjectTimeRepository : IRepository<SubjectTime>
{
    /// <summary>
    ///     Gets a time of a subject in a group
    /// </summary>
    /// <param name="subjectId">An id of subject that</param>
    /// <param name="groupId">An id of a group</param>
    /// <returns>A TimeSpan</returns>
    Task<TimeSpan?> GetTimeAsync(int subjectId, int groupId);
}