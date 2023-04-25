#nullable disable

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoList.Models
{
    public class College
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        public DateTime Deadline { get; set; }

        [ValidateNever]
        [Display(Name = "Group")]
        public int GroupId { get; set; }

        [ForeignKey(nameof(GroupId))]
        [ValidateNever]
        public Group Group { get; set; }

        [Display(Name = "Subject")]
        public int SubjectId { get; set; }

        [ForeignKey(nameof(SubjectId))]
        [ValidateNever]
        public Subject Subject { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        [ForeignKey(nameof(ApplicationUserId))]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "Show in everyone's home page")]
        public bool HomePageCollege { get; set; }


        public ICollection<Comment> Comments { get; set; }
        public ICollection<CompletedCollege> CompletedColleges { get; set; }
    }
}
