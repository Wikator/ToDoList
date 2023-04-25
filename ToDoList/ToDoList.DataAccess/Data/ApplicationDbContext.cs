using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDoList.Models;

namespace ToDoList.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<College> Colleges { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CompletedCollege> CompletedColleges { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<SubjectTime> SubjectTimes { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Group>().HasData(
                new Group { Id = 1, Name = "None", GroupType = GroupType.None },
                new Group { Id = 2, Name = "All groups", GroupType = GroupType.All }
                );

            builder.Entity<Subject>().HasData(
                new Subject { Id = 1, Name = "Inne", SubjectType = SubjectType.Other },
                new Subject { Id = 2, Name = "Język angielski", SubjectType = SubjectType.English }
			);
		}
	}
}
