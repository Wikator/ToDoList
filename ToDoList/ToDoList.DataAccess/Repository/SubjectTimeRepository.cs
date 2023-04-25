using Microsoft.EntityFrameworkCore;
using ToDoList.DataAccess.Data;
using ToDoList.DataAccess.Repository.IRepository;
using ToDoList.Models;

namespace ToDoList.DataAccess.Repository
{
    public class SubjectTimeRepository : Repository<SubjectTime>, ISubjectTimeRepository
    {
        public SubjectTimeRepository(ApplicationDbContext db) : base(db)
        {
        }

        public async Task<TimeSpan?> GetTimeAsync(int subjectId, int groupId)
        {
            SubjectTime? subjectTime = await dbSet.FirstOrDefaultAsync(x => x.SubjectId == subjectId && x.GroupId == groupId);

            if (subjectTime is null || subjectTime.Time is null)
                return null;

            return ((DateTime)subjectTime.Time).TimeOfDay;
        }
    }
}
