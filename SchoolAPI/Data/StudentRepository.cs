using Microsoft.EntityFrameworkCore;
using SchoolAPI.Models;

namespace SchoolAPI.Data
{
    public class StudentRepository : IStudentRepository
    {
        private readonly SchoolContext _context;
        public StudentRepository(SchoolContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Student>> GetStudentsAsync()
        {
            return await _context.Students.ToListAsync();
        }

        public async Task<Student> GetStudentAsync(int studentId)
        {
            return await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId);
        }

        public async Task<Student> AddStudentAsync(Student student)
        {
            var result = await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Student> UpdateStudentAsync(Student student)
        {
            var result = await _context.Students.FirstOrDefaultAsync(s => s.Id == student.Id);

            if(result is not null)
            {
                result.FirstName = student.FirstName;
                result.LastName = student.LastName;
                result.BirthDate = student.BirthDate;
                result.Grade = student.Grade;

                await _context.SaveChangesAsync();

                return result;
            }

            return null;
        }

        public async Task DeleteStudentAsync(int studentId)
        {
            var result = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId);
            if (result is not null)
            {
                _context.Students.Remove(result);
                await _context.SaveChangesAsync();
            }
        }
    }
}
