#nullable disable

#region

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using ToDoList.DataAccess.Repository.IRepository;
using ToDoList.Utility;

#endregion

namespace ToDoList.Web.Areas.Owner.Pages.RoleManager;

[Authorize(Policy = "RequireOwner")]
[Area("Owner")]
public class EditUserRoleModel : PageModel
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<IdentityUser> _userManager;

    public EditUserRoleModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
    }

    [BindProperty] public InputModel Input { get; set; }


    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id is null)
            RedirectToPage("./UserRoleManager");

        var identityUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);

        if (identityUser is null)
            RedirectToPage("./UserRoleManager");

        var applicationUser = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(x => x.Id == identityUser.Id);

        if (await _userManager.IsInRoleAsync(identityUser, SD.RoleUserOwner))
            return RedirectToPage("./UserRoleManager");

        var roles = await _userManager.GetRolesAsync(identityUser);

        Input = new InputModel
        {
            IdentityUser = identityUser,
            Roles = _roleManager.Roles.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Name
            }).Where(x => x.Value != SD.RoleUserOwner),
            Role = roles[0],
            Id = identityUser.Id,
            UserName = applicationUser.Nickname
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Input.Id is null)
            RedirectToPage("./UserRoleManager");

        var identityUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == Input.Id);

        if (identityUser is null)
            RedirectToPage("./UserRoleManager");

        if (await _userManager.IsInRoleAsync(identityUser, SD.RoleUserOwner))
            return RedirectToPage("./UserRoleManager");

        if (!ModelState.IsValid) return Page();
        {
            var roles = await _userManager.GetRolesAsync(identityUser);

            if (Input.Role != SD.RoleUserOwner && Input.Role != SD.RoleUserAdmin)
            {
                var colleges =
                    await _unitOfWork.College.GetCollegesAsync(false, x => x.ApplicationUserId == identityUser.Id);

                foreach (var college in colleges)
                {
                    college.HomePageCollege = false;
                    _unitOfWork.College.Update(college);
                }

                await _unitOfWork.SaveAsync();
            }

            if (roles[0] == Input.Role) return RedirectToPage("./UserRoleManager");
            
            await _userManager.RemoveFromRoleAsync(identityUser, roles[0]);
            await _userManager.AddToRoleAsync(identityUser, Input.Role);

            return RedirectToPage("./UserRoleManager");
        }

    }

    public class InputModel
    {
        [ValidateNever] public IdentityUser IdentityUser { get; set; }

        [ValidateNever] public IEnumerable<SelectListItem> Roles { get; set; }

        [Required] public string Role { get; set; }

        [Required] public string Id { get; set; }

        [ValidateNever] public string UserName { get; set; }
    }
}