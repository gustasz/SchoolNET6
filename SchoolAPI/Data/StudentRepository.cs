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
            return await _context.Students.AsNoTracking().ToListAsync();
        }

        public async Task<Student> GetStudentAsync(int studentId)
        {
            return await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Id == studentId);
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

            if (result is not null)
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

        public async Task<IEnumerable<Course>> GetStudentCoursesAsync(int studentId)
        {
            var student = await _context.Students.AsNoTracking()
                .Include(s => s.Courses).ThenInclude(c => c.Teacher)
                .Include(s => s.Courses).ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(s => s.Id == studentId);
            if (student is not null)
            {
                var courses = student.Courses;
                return courses;
            }
            return null;
        }

        public async Task<Student> AddCourseToStudent(int studentId, int courseId)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId);

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
            if (course is null)
            {
                return null; // TODO
            }

            if (student.Courses is null)
            {
                student.Courses = new List<Course>();
            }

            student.Courses.Add(course);

            await _context.SaveChangesAsync();

            return student;
        }

        public async Task DeleteCourseFromStudent(int studentId, int courseId)
        {
            var student = await _context.Students.Include(s => s.Courses).FirstOrDefaultAsync(s => s.Id == studentId);
            if (student.Courses is not null)
            {
                var course = student.Courses.FirstOrDefault(c => c.Id == courseId);

                if (course is not null)
                {
                    student.Courses.Remove(course);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
