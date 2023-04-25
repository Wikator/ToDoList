#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoList.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Nickname { get; set; }

        [Display(Name = "Group1")]
        public int Group1Id { get; set; }

        [ForeignKey("Group1Id")]
        [ValidateNever]
		[InverseProperty("ApplicationUsers1")]
		public Group Group1 { get; set; }

        [Display(Name = "Group2")]
        public int Group2Id { get; set; }

        [ForeignKey("Group2Id")]
        [ValidateNever]
		[InverseProperty("ApplicationUsers2")]
		public Group Group2 { get; set; }


        public ICollection<College> Colleges { get; set; }
        public ICollection<CompletedCollege> CompletedColleges { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
