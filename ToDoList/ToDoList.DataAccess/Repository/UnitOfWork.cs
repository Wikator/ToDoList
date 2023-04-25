using ToDoList.DataAccess.Data;
using ToDoList.DataAccess.Repository.IRepository;
using ToDoList.Models;

namespace ToDoList.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            CompletedCollege = new Repository<CompletedCollege>(_db);
            Comment = new Repository<Comment>(_db);
            ApplicationUser = new Repository<ApplicationUser>(_db);
            Subject = new Repository<Subject>(_db);
            Group = new Repository<Group>(_db);
            College = new CollegeRepository(_db, this);
            SubjectTime = new SubjectTimeRepository(_db);
        }

        public IRepository<CompletedCollege> CompletedCollege { get; private set; }

        public IRepository<Comment> Comment { get; private set; }

        public IRepository<ApplicationUser> ApplicationUser { get; private set; }

        public IRepository<Subject> Subject { get; private set; }

        public IRepository<Group> Group { get; private set; }

        public ICollegeRepository College { get; private set; }

        public ISubjectTimeRepository SubjectTime { get; private set; }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
