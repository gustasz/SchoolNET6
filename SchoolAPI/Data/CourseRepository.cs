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
            return await _context.Courses.Include(s => s.Subject).Include(t => t.Teacher).ToListAsync();
        }

        public async Task<Course> GetCourseAsync(int courseId)
        {
            return await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
        }

        public async Task<Course> AddCourseAsync(Course course)
        {
            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == course.Subject.Id);
            if(subject == null)
            {
                return null; // TODO
            }
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == course.Teacher.Id);
            if(teacher == null)
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

                //_context.Entry(result.Subject).CurrentValues.SetValues(subject);
                //_context.Entry(result.Teacher).CurrentValues.SetValues(teacher);

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
    }
}
