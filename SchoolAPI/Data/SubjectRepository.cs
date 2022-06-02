using Microsoft.EntityFrameworkCore;
using SchoolAPI.Models;

namespace SchoolAPI.Data
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly SchoolContext _context;
        public SubjectRepository(SchoolContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Subject>> GetSubjectsAsync()
        {
            return await _context.Subjects.ToListAsync();
        }

        public async Task<Subject> GetSubjectAsync(int subjectId)
        {
            return await _context.Subjects.FirstOrDefaultAsync(s => s.Id == subjectId);
        }

        public async Task<Subject> AddSubjectAsync(Subject subject)
        {
            var result = await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Subject> UpdateSubjectAsync(Subject subject)
        {
            var result = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == subject.Id);

            if (result is not null)
            {
                result.Name = subject.Name;

                await _context.SaveChangesAsync();

                return result;
            }

            return null;
        }

        public async Task DeleteSubjectAsync(int subjectId)
        {
            var result = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == subjectId);
            _context.Subjects.Remove(result);
            await _context.SaveChangesAsync();
        }
    }
}
