#nullable disable

using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public SubjectType SubjectType { get; set; }

        public ICollection<College> Colleges { get; set; }

        public ICollection<SubjectTime> SubjectTimes { get; set; }
    }


    public enum SubjectType
    {
		English,
		Standard,
        Other
	}
}
