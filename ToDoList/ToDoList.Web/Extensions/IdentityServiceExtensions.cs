using Microsoft.AspNetCore.Identity;
using ToDoList.DataAccess.Data;
using ToDoList.Utility;

namespace ToDoList.Web.Extensions
{
	public static class IdentityServiceExtensions
	{
		public static IServiceCollection AddIdentityServices(this IServiceCollection services)
		{
			services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders()
				.AddDefaultUI()
				.AddEntityFrameworkStores<ApplicationDbContext>();

			services.AddAuthorization(options =>
			{
				options.AddPolicy("RequireAdmin", policy => policy.RequireRole(SD.Role_User_Admin, SD.Role_User_Owner));
				options.AddPolicy("RequireOwner", policy => policy.RequireRole(SD.Role_User_Owner));
			});

			return services;
		}
	}
}
