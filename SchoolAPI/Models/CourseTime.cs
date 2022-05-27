namespace SchoolAPI.Models
{
    public class CourseTime
    {
        public int Id { get; set; }
        public DateTimeOffset Time { get; set; }
        public Course Course { get; set; }
    }
}
