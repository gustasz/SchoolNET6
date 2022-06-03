using SchoolAPI.Models;

namespace SchoolAPI.Data
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetCoursesAsync();
        Task<Course> GetCourseAsync(int courseId);
        Task<Course> AddCourseAsync(Course course);
        Task<Course> UpdateCourseAsync(Course course);
        Task DeleteCourseAsync(int courseId);
        Task<IEnumerable<Student>> GetCourseStudentsAsync(int courseId);
        Task<Course> AddStudentToCourseAsync(int courseId, int studentId);
        Task DeleteStudentFromCourseAsync(int courseId, int studentId);
    }
}
