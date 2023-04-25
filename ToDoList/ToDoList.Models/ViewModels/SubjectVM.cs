#nullable disable

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ToDoList.Models.ViewModels
{
    public class SubjectVM
    {
        public Subject Subject { get; set; }

        [ValidateNever]
        public SubjectTime[] SubjectTimes { get; set; }
    }
}
