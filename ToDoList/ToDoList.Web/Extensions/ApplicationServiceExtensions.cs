using ToDoList.DataAccess.Data;
using ToDoList.DataAccess.DbInitializer;
using ToDoList.DataAccess.Repository.IRepository;
using ToDoList.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;

namespace ToDoList.Web.Extensions
{
	public static class ApplicationServiceExtensions
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services,
			IConfiguration config)
		{
			services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
			});

			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddScoped<IDbInitializer, DbInitializer>();

			services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = "/Identity/Account/Login";
				options.LogoutPath = "/Identity/Account/Logout";
				options.AccessDeniedPath = "/Identity/Account/AccessDenied";
			});

			return services;
		}
	}
}
