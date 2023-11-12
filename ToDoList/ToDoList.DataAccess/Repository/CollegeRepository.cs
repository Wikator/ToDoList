#region

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ToDoList.DataAccess.Data;
using ToDoList.DataAccess.Repository.IRepository;
using ToDoList.Models;

#endregion

namespace ToDoList.DataAccess.Repository;

public class CollegeRepository : Repository<College>, ICollegeRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public CollegeRepository(DbContext db, IUnitOfWork unitOfWork) : base(db)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<College>> GetCollegesAsync(bool ignoreNoneGroups,
        ApplicationUser? applicationUser = null, Expression<Func<College, bool>>? filter = null)
    {
        var colleges = await GetAllAsync(filter, "Subject,Group");

        return await GetFilteredColleges(colleges, ignoreNoneGroups, applicationUser);
    }

    public async Task<IEnumerable<College>> GetCollegesAsync<TDataType>(bool ignoreNoneGroups,
        Expression<Func<College, TDataType>> order, ApplicationUser? applicationUser = null,
        Expression<Func<College, bool>>? filter = null)
    {
        var colleges =
            await GetAllAsync(filter: filter, order: order, includeProperties: "Subject,Group");

        return await GetFilteredColleges(colleges, ignoreNoneGroups, applicationUser);
    }


    public void RemoveCollege(College college, string wwwRootPath)
    {
        _unitOfWork.CompletedCollege.RemoveRange(college.CompletedColleges);

        foreach (var comment in college.Comments)
        {
            if (comment.FileUrl is not null)
            {
                var oldImagePath = Path.Combine(wwwRootPath, comment.FileUrl.TrimStart('\\'));
                if (File.Exists(oldImagePath)) File.Delete(oldImagePath);
            }

            _unitOfWork.Comment.Remove(comment);
        }

        Remove(college);
    }


    private async Task<IEnumerable<College>> GetFilteredColleges(IEnumerable<College> colleges, bool ignoreNoneGroups,
        ApplicationUser? applicationUser)
    {
        var noneGroup = await _unitOfWork.Group.GetFirstOrDefaultAsync(x => x.GroupType == GroupType.None) ??
                        throw new Exception();

        if (applicationUser is null)
        {
            colleges = colleges.Where(x => x.HomePageCollege);

            if (ignoreNoneGroups) colleges = colleges.Where(x => x.GroupId != noneGroup.Id);
        }
        else
        {
            colleges = colleges.Where(x => x.ApplicationUser == applicationUser || x.HomePageCollege);

            if (ignoreNoneGroups)
                colleges = colleges.Where(x => x.GroupId != noneGroup.Id || x.ApplicationUser == applicationUser);
        }

        return colleges;
    }
}