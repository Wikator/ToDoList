#nullable disable

#region

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

#endregion

namespace ToDoList.Models.ViewModels;

public class CollegeVM
{
    public College ListItem { get; set; }

    [ValidateNever] public DateTime? Time { get; set; }

    [ValidateNever] public IEnumerable<SelectListItem> Subjects { get; set; }

    [ValidateNever] public IEnumerable<SelectListItem> Groups1 { get; set; }

    [ValidateNever] public IEnumerable<SelectListItem> Groups2 { get; set; }

    [ValidateNever] public IEnumerable<SelectListItem> Groups3 { get; set; }

    [Display(Name = "Group")] public int Group1Id { get; set; }

    [Display(Name = "Group")] public int Group2Id { get; set; }

    [Display(Name = "Group")] public int Group3Id { get; set; }

    // [ValidateNever]
    //  public Dictionary<string, Dictionary<string, string>> Times { get; set; }
}