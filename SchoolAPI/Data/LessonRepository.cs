using Microsoft.EntityFrameworkCore;
using SchoolAPI.Models;

namespace SchoolAPI.Data
{
    public class LessonRepository : ILessonRepository
    {
        private readonly SchoolContext _context;
        public LessonRepository(SchoolContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Lesson>> GetLessonsAsync()
        {
            return await _context.Lessons.AsNoTracking().Include(l => l.Course).ToListAsync();
        }

        public async Task<Lesson> GetLessonAsync(int lessonId)
        {
            return await _context.Lessons.AsNoTracking().Include(l => l.Course).FirstOrDefaultAsync(l => l.Id == lessonId);
        }

        public async Task<Lesson> AddLessonAsync(Lesson lesson)
        {
            //var result = await _context.Lessons.AddAsync(lesson);
            var course = await _context.Courses.Include(c => c.Lessons).FirstOrDefaultAsync(c => c.Id == lesson.Course.Id);
            if (course is not null)
            {
                
                course.Lessons.Add(lesson);
                await _context.SaveChangesAsync();
                var result = await _context.Lessons.AsNoTracking().FirstOrDefaultAsync(c => c.Id == course.Id);
                return result;
            }
            return null;
        }

        public async Task<Lesson> UpdateLessonAsync(Lesson lesson)
        {
            var result = await _context.Lessons.FirstOrDefaultAsync(l => l.Id == lesson.Id);

            if (result is not null)
            {
                result.Time = lesson.Time;
                result.Course = lesson.Course;

                await _context.SaveChangesAsync();

                return result;
            }

            return null;
        }
        public async Task DeleteLessonAsync(int lessonId)
        {
            var result = await _context.Lessons.FirstOrDefaultAsync(l => l.Id == lessonId);
            if (result is not null)
            {
                _context.Lessons.Remove(result);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Lesson>> GetCourseLessonsAsync(int courseId)
        {
            return await _context.Lessons.AsNoTracking().Include(l => l.Course).Where(l => l.Course.Id == courseId).ToListAsync();
        }

        public async Task<Course> CreateLessonsForCourseAsync(Lesson[] lessons)
        {
            var course = await _context.Courses.Include(c => c.Lessons).FirstOrDefaultAsync(c => c.Id == lessons[0].Course.Id);

            if (course is null)
            {
                return null;
            }

            if (course.Lessons is null)
            {
                course.Lessons = new List<Lesson>();
            }


            foreach (var lesson in lessons)
            {
                course.Lessons.Add(lesson);
            }

            await _context.SaveChangesAsync();

            return course;
        }
    }
}
