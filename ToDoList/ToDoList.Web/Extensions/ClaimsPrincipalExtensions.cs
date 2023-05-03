using System.Security.Claims;

namespace ToDoList.Web.Extensions
{
	public static class ClaimsPrincipalExtensions
	{
		public static string GetLoggedInUserId(this ClaimsPrincipal user)
		{
			ClaimsIdentity claimsIdentity = (user.Identity as ClaimsIdentity)!;
			Claim claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!;
			return claim.Value;
		}
	}
}
