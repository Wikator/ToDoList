#nullable disable

#region

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

#endregion

namespace ToDoList.Models;

public class SubjectTime
{
    [Key] public int Id { get; set; }

    public int SubjectId { get; set; }

    [ValidateNever]
    [ForeignKey(nameof(SubjectId))]
    public Subject Subject { get; set; }

    public int GroupId { get; set; }

    [ValidateNever]
    [ForeignKey(nameof(GroupId))]
    public Group Group { get; set; }

    [DataType(DataType.Time)] public DateTime? Time { get; set; }
}