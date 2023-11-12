#nullable disable

#region

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

#endregion

namespace ToDoList.Models.ViewModels;

public class SubjectVM
{
    public Subject Subject { get; set; }

    [ValidateNever] public SubjectTime[] SubjectTimes { get; set; }
}