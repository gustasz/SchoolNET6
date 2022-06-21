using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SchoolAPI.Data.Repositories;
using SchoolAPI.Models;

namespace SchoolAPI.Data
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {

        public CourseRepository(SchoolContext _context) : base(_context) { }

        public new async Task<IEnumerable<Course>> GetAllAsync()
        {
            var courseList = await _context.Courses.AsNoTracking().Include(c => c.Subject).Include(c => c.Teacher).Include(c => c.Students).Include(c => c.Lessons).ToListAsync();
            return courseList;
        }

        public new async Task<Course> GetByIdAsync(int id)
        {
            return await _context.Courses.AsNoTracking().Include(c => c.Subject).Include(c => c.Teacher).Include(c => c.Students).Include(c => c.Lessons).FirstOrDefaultAsync(c => c.Id == id);
        }

        /*public async Task<IEnumerable<Course>> GetCoursesAsync()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.Courses, out List<Course> courseList))
            {
                courseList = await _context.Courses.AsNoTracking().Include(c => c.Subject).Include(c => c.Teacher).Include(c => c.Students).Include(c => c.Lessons).ToListAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));

                _memoryCache.Set(CacheKeys.Courses, courseList, cacheEntryOptions);
            }
            return courseList;
        }

        public async Task<Course> GetCourseAsync(int courseId)
        {
            return await _context.Courses.AsNoTracking().Include(c => c.Subject).Include(c => c.Teacher).Include(c => c.Students).Include(c => c.Lessons).FirstOrDefaultAsync(c => c.Id == courseId);
        }

        public async Task<Course> AddCourseAsync(Course course)
        {
            var result = await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
            _memoryCache.Remove(CacheKeys.Courses);
            return result.Entity;
        }

        public async Task<Course> UpdateCourseAsync(Course course)
        {
            var result = await _context.Courses.FirstOrDefaultAsync(c => c.Id == course.Id);

            result.Subject = course.Subject;
            result.Teacher = course.Teacher;

            await _context.SaveChangesAsync();
            _memoryCache.Remove(CacheKeys.Courses);

            return result;
        }

        public async Task DeleteCourseAsync(int courseId)
        {
            var result = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
            _context.Courses.Remove(result);
            await _context.SaveChangesAsync();
            _memoryCache.Remove(CacheKeys.Courses);
        }*/

        public async Task<IEnumerable<Student>> GetCourseStudentsAsync(int courseId)
        {
            var course = await _context.Courses.AsNoTracking().Include(s => s.Students).FirstOrDefaultAsync(c => c.Id == courseId);

            var students = course.Students;
            return students;

        }

        public async Task<Course> AddStudentToCourseAsync(int courseId, int studentId)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId);

            if (course.Students is null)
            {
                course.Students = new List<Student>();
            }

            course.Students.Add(student);

            await _context.SaveChangesAsync();

            return course;
        }

        public async Task DeleteStudentFromCourseAsync(int courseId, int studentId)
        {
            var course = await _context.Courses.Include(c => c.Students).FirstOrDefaultAsync(c => c.Id == courseId);
            var student = course.Students.FirstOrDefault(s => s.Id == studentId);

            course.Students.Remove(student);
            await _context.SaveChangesAsync();

        }

        public async Task DeleteClassFromCourseAsync(int courseId, int gradeNum, int classNum)
        {
            var course = await _context.Courses.Include(c => c.Students).FirstOrDefaultAsync(c => c.Id == courseId);
            var students = course.Students.Where(s => s.Grade == gradeNum && s.Class == classNum).ToList();

            foreach (var student in students)
            {
                course.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
        }
    }
}
