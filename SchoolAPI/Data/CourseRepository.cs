using Microsoft.EntityFrameworkCore;
using SchoolAPI.Models;

namespace SchoolAPI.Data
{
    public class CourseRepository : ICourseRepository
    {
        private readonly SchoolContext _context;
        public CourseRepository(SchoolContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Course>> GetCoursesAsync()
        {
            return await _context.Courses.AsNoTracking().Include(s => s.Subject).Include(t => t.Teacher).ToListAsync();
        }

        public async Task<Course> GetCourseAsync(int courseId)
        {
            return await _context.Courses.AsNoTracking().Include(s => s.Subject).Include(t => t.Teacher).FirstOrDefaultAsync(c => c.Id == courseId);
        }

        public async Task<Course> AddCourseAsync(Course course)
        {
            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == course.Subject.Id);
            if (subject == null)
            {
                return null; // TODO
            }
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == course.Teacher.Id);
            if (teacher == null)
            {
                return null;
            }

            course.Subject = subject;
            course.Teacher = teacher;

            var result = await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Course> UpdateCourseAsync(Course course)
        {
            var result = await _context.Courses.FirstOrDefaultAsync(c => c.Id == course.Id);

            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == course.Subject.Id);
            if (subject == null)
            {
                return null; // TODO
            }
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == course.Teacher.Id);
            if (teacher == null)
            {
                return null;
            }

            result.Subject = subject;
            result.Teacher = teacher;

            await _context.SaveChangesAsync();

            return result;
        }

        public async Task DeleteCourseAsync(int courseId)
        {
            var result = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
            if (result is not null)
            {
                _context.Courses.Remove(result);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Student>> GetCourseStudentsAsync(int courseId)
        {
            var course = await _context.Courses.AsNoTracking().Include(s => s.Students).FirstOrDefaultAsync(c => c.Id == courseId);

            if (course is not null)
            {
                var students = course.Students;
                return students;
            }

            return null; //todo
        }

        public async Task<Course> AddStudentToCourse(int courseId, int studentId)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null)
            {
                return null; // TODO
            }

            if (course.Students is null)
            {
                course.Students = new List<Student>();
            }

            course.Students.Add(student);

            await _context.SaveChangesAsync();

            return course;
        }

        //DELETE /courses/<course-id>/students/<student-id>
        public async Task DeleteStudentFromCourse(int courseId, int studentId)
        {
            var course = await _context.Courses.Include(c => c.Students).FirstOrDefaultAsync(c => c.Id == courseId);
            if (course.Students is not null)
            {
                var student = course.Students.FirstOrDefault(s => s.Id == studentId);

                if (student is not null)
                {
                    course.Students.Remove(student);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
