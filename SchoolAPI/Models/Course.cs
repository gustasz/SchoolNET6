namespace SchoolAPI.Models
{
    public class Course
    {
        public int Id { get; set; }
        
        public Subject Subject { get; set; }
        public Teacher Teacher { get; set; }
        public ICollection<CourseTime> CourseTimes { get; set; }

        public ICollection<Student> Students { get; set; }
    }
}
