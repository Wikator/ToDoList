#region

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoList.DataAccess.Data;
using ToDoList.DataAccess.Repository.IRepository;
using ToDoList.Models;
using ToDoList.Utility;

#endregion

namespace ToDoList.DataAccess.DbInitializer;

public class DbInitializer : IDbInitializer
{
    private readonly ApplicationDbContext _db;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<IdentityUser> _userManager;

    public DbInitializer(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext db,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
        _unitOfWork = unitOfWork;
    }

    public async Task InitializeAsync()
    {
        try
        {
            if ((await _db.Database.GetPendingMigrationsAsync()).Any()) await _db.Database.MigrateAsync();
        }
        catch (Exception)
        {
            // ignored
        }

        if (!_roleManager.RoleExistsAsync(SD.RoleUserOwner).GetAwaiter().GetResult())
        {
            _roleManager.CreateAsync(new IdentityRole(SD.RoleUserOwner)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.RoleUserAdmin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.RoleUserCustomer)).GetAwaiter().GetResult();


            await _unitOfWork.SaveAsync();

            var group = await _unitOfWork.Group.GetFirstOrDefaultAsync(x => x.GroupType == GroupType.None) ??
                        throw new Exception("Group not found");

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "wiktor@szymulewicz.com",
                Email = "wiktor@szymulewicz.com",
                Group1Id = group.Id,
                Group2Id = group.Id,
                Nickname = "Wikator"
            }, "Test,123").GetAwaiter().GetResult();

            var user = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(
                u => u.Email == "wiktor@szymulewicz.com");

            if (user != null) _userManager.AddToRoleAsync(user, SD.RoleUserOwner).GetAwaiter().GetResult();
        }
    }
}