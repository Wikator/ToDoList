#region

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDoList.Models;

#endregion

namespace ToDoList.DataAccess.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public required DbSet<College> Colleges { get; set; }
    public required DbSet<Comment> Comments { get; set; }
    public required DbSet<CompletedCollege> CompletedColleges { get; set; }
    public required DbSet<Subject> Subjects { get; set; }
    public required DbSet<SubjectTime> SubjectTimes { get; set; }
    public required DbSet<Group> Groups { get; set; }
    public required DbSet<ApplicationUser> ApplicationUsers { get; set; }


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