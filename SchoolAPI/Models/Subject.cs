using System.ComponentModel.DataAnnotations;

namespace SchoolAPI.Models
{
    public class Subject
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public ICollection<Course> Courses { get; set; }
    }
}
