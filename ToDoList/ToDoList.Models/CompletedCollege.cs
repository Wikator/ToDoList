#nullable disable

#region

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

#endregion

namespace ToDoList.Models;

public class CompletedCollege
{
    [Key] public int Id { get; set; }

    public int CollegeId { get; set; }

    [ForeignKey("CollegeId")]
    [ValidateNever]
    public College College { get; set; }

    [Required] public string ApplicationUserId { get; set; }

    [ForeignKey("ApplicationUserId")]
    [ValidateNever]
    public ApplicationUser ApplicationUser { get; set; }
}