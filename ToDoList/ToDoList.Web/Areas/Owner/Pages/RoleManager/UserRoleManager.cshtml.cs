#nullable disable

#region

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

#endregion

namespace ToDoList.Web.Areas.Owner.Pages.RoleManager;

[Authorize(Policy = "RequireOwner")]
[Area("Owner")]
public class UserRoleManagerModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    public UserRoleManagerModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public IEnumerable<InputModel> Input { get; set; }


    public async Task<IActionResult> OnGetAsync()
    {
        IEnumerable<IdentityUser> users = await _userManager.Users.ToListAsync();

        Input = users.Select(u =>
        {
            var userRoles = _userManager.GetRolesAsync(u);

            return new InputModel
            {
                IdentityUser = u,
                Role = userRoles.Result[0]
            };
        });

        return Page();
    }

    public class InputModel
    {
        public IdentityUser IdentityUser { get; set; }
        public string Role { get; set; }
    }
}