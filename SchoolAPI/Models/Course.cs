using System.ComponentModel.DataAnnotations;

namespace SchoolAPI.Models
{
    public class Course
    {
        public int Id { get; set; }
        [Required,Range(1,12)]
        public int ForGrade { get; set; }
        public int ForClass { get; set; } // 0 if students fron any class in that grade (a,b,c) can be in the course
        
        public Subject Subject { get; set; }
        public Teacher Teacher { get; set; }

        public ICollection<Lesson> Lessons { get; set; }
        public ICollection<Student> Students { get; set; }
    }
}
