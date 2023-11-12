#nullable disable

#region

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

#endregion

namespace ToDoList.Models;

public class Comment
{
    [Key] public int Id { get; set; }

    public int CollegeId { get; set; }

    [ValidateNever]
    [ForeignKey(nameof(CollegeId))]
    public College College { get; set; }

    [Required] public string ApplicationUserId { get; set; }

    [ValidateNever]
    [ForeignKey(nameof(ApplicationUserId))]
    public ApplicationUser ApplicationUser { get; set; }

    [Required] [MaxLength(1000)] public string Text { get; set; }

    [Display(Name = "File")] public string FileUrl { get; set; }
}