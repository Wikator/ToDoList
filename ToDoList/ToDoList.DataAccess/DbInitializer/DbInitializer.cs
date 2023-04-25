using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoList.DataAccess.Data;
using ToDoList.DataAccess.Repository.IRepository;
using ToDoList.Models;
using ToDoList.Utility;

namespace ToDoList.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;

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
                if (_db.Database.GetPendingMigrations().Any())
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception)
            {

            }
            
            if (!_roleManager.RoleExistsAsync(SD.Role_User_Owner).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Owner)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Customer)).GetAwaiter().GetResult();

                
                await _unitOfWork.SaveAsync();

                Group group = await _unitOfWork.Group.GetFirstOrDefaultAsync(x => x.GroupType == GroupType.None) ?? throw new Exception("Group not found");

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "wiktor@szymulewicz.com",
                    Email = "wiktor@szymulewicz.com",
                    Group1Id = group.Id,
                    Group2Id = group.Id,
                    Nickname = "Wikator"
                }, "Test,123").GetAwaiter().GetResult();

                ApplicationUser? user = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.Email == "wiktor@szymulewicz.com");

                if (user != null)
                {
                    _userManager.AddToRoleAsync(user, SD.Role_User_Owner).GetAwaiter().GetResult();
                }
            }
            return;
        }

    }
}
