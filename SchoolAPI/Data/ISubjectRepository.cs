using SchoolAPI.Models;

namespace SchoolAPI.Data
{
    public interface ISubjectRepository
    {
        Task<IEnumerable<Subject>> GetSubjectsAsync();
        Task<Subject> GetSubjectAsync(int subjectId);
        Task<Subject> AddSubjectAsync(Subject subject);
        Task<Subject> UpdateSubjectAsync(Subject subject);
        Task DeleteSubjectAsync(int subjectId);
    }
}
