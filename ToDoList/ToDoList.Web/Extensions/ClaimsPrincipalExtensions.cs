#region

using System.Security.Claims;

#endregion

namespace ToDoList.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetLoggedInUserId(this ClaimsPrincipal user)
    {
        var claimsIdentity = (user.Identity as ClaimsIdentity)!;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!;
        return claim.Value;
    }
}