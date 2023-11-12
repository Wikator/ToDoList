#region

using Microsoft.EntityFrameworkCore;
using ToDoList.DataAccess.Data;
using ToDoList.DataAccess.Repository.IRepository;
using ToDoList.Models;

#endregion

namespace ToDoList.DataAccess.Repository;

public class SubjectTimeRepository : Repository<SubjectTime>, ISubjectTimeRepository
{
    public SubjectTimeRepository(ApplicationDbContext db) : base(db)
    {
    }

    public async Task<TimeSpan?> GetTimeAsync(int subjectId, int groupId)
    {
        var subjectTime = await DbSet.FirstOrDefaultAsync(x => x.SubjectId == subjectId && x.GroupId == groupId);

        return subjectTime?.Time?.TimeOfDay;
    }
}