using System.ComponentModel.DataAnnotations;

namespace SchoolAPI.Models
{
    public class Student
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        [Range(1,12)]
        public int Grade { get; set; }
        [Range(0, 4)]
        public int Class { get; set; } // 0 if the school has only 1 class in that grade, otherwise 1 = A, 2 = B, 3 = C, 4 = D

        public ICollection<Course> Courses { get; set; }
    }
}
