using SchoolAPI.Models;

namespace SchoolAPI.Data
{
    public interface ITeacherRepository
    {
        Task<IEnumerable<Teacher>> GetTeachersAsync();
        Task<Teacher> GetTeacherAsync(int teacherId);
        Task<Teacher> AddTeacherAsync(Teacher teacher);
        Task<Teacher> UpdateTeacherAsync(Teacher teacher);
        Task DeleteTeacherAsync(int teacherId);
    }
}
