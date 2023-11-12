#region

using ToDoList.Models;

#endregion

namespace ToDoList.DataAccess.Repository.IRepository;

public interface IUnitOfWork
{
    IRepository<CompletedCollege> CompletedCollege { get; }

    IRepository<Comment> Comment { get; }

    IRepository<ApplicationUser> ApplicationUser { get; }

    IRepository<Subject> Subject { get; }

    IRepository<Group> Group { get; }

    ICollegeRepository College { get; }

    ISubjectTimeRepository SubjectTime { get; }

    Task SaveAsync();
}