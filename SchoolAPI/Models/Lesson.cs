namespace SchoolAPI.Models
{
    public class Lesson
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public Course Course { get; set; }
    }
}
