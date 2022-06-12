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
        [Required,Range(1,12)]
        public int Grade { get; set; }
        public int Class { get; set; } // 0 if there's only one class in the whole grade, otherwise 1 = a, 2 = b etc.

        public ICollection<Course> Courses { get; set; }
    }
}
