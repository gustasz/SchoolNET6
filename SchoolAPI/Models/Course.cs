using System.ComponentModel.DataAnnotations;

namespace SchoolAPI.Models
{
    public class Course
    {
        public int Id { get; set; }
        [Range(1,12)]
        public int ForGrade { get; set; }
        [Range(0,4)]
        public int ForClass { get; set; } // 0 if students from any class in that grade (a,b,c) can be in the course, otherwise 1 = only a class students can be in this course and etc.
        
        public Subject Subject { get; set; }
        public Teacher Teacher { get; set; }

        public ICollection<Lesson> Lessons { get; set; }
        public ICollection<Student> Students { get; set; }
    }
}
