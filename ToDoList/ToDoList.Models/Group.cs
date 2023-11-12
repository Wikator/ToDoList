#nullable disable

#region

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace ToDoList.Models;

public class Group
{
    [Key] public int Id { get; set; }

    [Required] public string Name { get; set; }

    public GroupType GroupType { get; set; }

    public ICollection<College> Colleges { get; set; }
    public ICollection<SubjectTime> SubjectTimes { get; set; }

    [InverseProperty("Group1")] public ICollection<ApplicationUser> ApplicationUsers1 { get; set; }

    [InverseProperty("Group2")] public ICollection<ApplicationUser> ApplicationUsers2 { get; set; }
}

public enum GroupType
{
    English,
    Standard,
    None,
    All
}