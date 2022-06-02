using System.ComponentModel.DataAnnotations;

namespace SchoolAPI.Models
{
    public class Student
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; } // add middle names?
        public DateTime BirthDate { get; set; }
        [Required,Range(1,13)]
        public int Grade { get; set; }

        public ICollection<Course> Courses { get; set; }
    }
}
