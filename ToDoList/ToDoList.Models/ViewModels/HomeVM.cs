#nullable disable

#region

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

#endregion

namespace ToDoList.Models.ViewModels;

public class HomeVM
{
    [ValidateNever] public CollegeLists AllCollegeList { get; set; }

    [ValidateNever] public CollegeLists GroupCollegeList { get; set; }

    [ValidateNever] public Group Group1 { get; set; }

    [ValidateNever] public Group Group2 { get; set; }

    [ValidateNever] public ApplicationUser ApplicationUser { get; set; }


    public class CollegeLists
    {
        [ValidateNever] public IEnumerable<College> CollegePastDeadlineList { get; set; }

        [ValidateNever] public IEnumerable<College> CollegeList { get; set; }

        [ValidateNever] public IEnumerable<College> CompletedCollegeList { get; set; }
    }
}