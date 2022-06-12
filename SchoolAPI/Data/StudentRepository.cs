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
            return await _context.Students.AsNoTracking().Include(s => s.Courses).FirstOrDefaultAsync(s => s.Id == studentId);
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

            result.FirstName = student.FirstName;
            result.LastName = student.LastName;
            result.BirthDate = student.BirthDate;
            result.Grade = student.Grade;

            await _context.SaveChangesAsync();

            return result;

        }

        public async Task DeleteStudentAsync(int studentId)
        {
            var result = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId);
            _context.Students.Remove(result);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Course>> GetStudentCoursesAsync(int studentId)
        {
            var student = await _context.Students.AsNoTracking()
                .Include(s => s.Courses).ThenInclude(c => c.Teacher)
                .Include(s => s.Courses).ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            var courses = student.Courses;
            return courses;
        }

        public async Task<Student> AddCourseToStudentAsync(int studentId, int courseId)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId);

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);

            if (student.Courses is null)
            {
                student.Courses = new List<Course>();
            }

            student.Courses.Add(course);

            await _context.SaveChangesAsync();

            return student;
        }

        public async Task DeleteCourseFromStudentAsync(int studentId, int courseId)
        {
            var student = await _context.Students.Include(s => s.Courses).FirstOrDefaultAsync(s => s.Id == studentId);
            var course = student.Courses.FirstOrDefault(c => c.Id == courseId);

            student.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Student>> GetStudentsFromClassAsync(int gradeId, int classId)
        {
            return await _context.Students.AsNoTracking().Where(s => s.Grade == gradeId).Where(s => s.Class == classId).ToListAsync();
        }

        public async Task<IEnumerable<Student>> AddStudentsToCourseAsync(Student[] students, int courseId)
        {
            var course = await _context.Courses.Include(c => c.Students).FirstOrDefaultAsync(c => c.Id == courseId);

            if (course.Students is null)
            {
                course.Students = new List<Student>();
            }

            foreach (var student in students)
            {
                course.Students.Add(student);
            }

            await _context.SaveChangesAsync();

            return course.Students;
        }
    }
}
