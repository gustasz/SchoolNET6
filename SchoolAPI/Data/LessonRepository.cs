﻿using Microsoft.EntityFrameworkCore;
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

        /*public async Task<Lesson> AddLessonAsync(Lesson lesson)
        {
            //var result = await _context.Lessons.AddAsync(lesson);
            var course = await _context.Courses.Include(c => c.Lessons).FirstOrDefaultAsync(c => c.Id == lesson.Course.Id);
            if (course is not null)
            {
                if(course.Lessons is null)
                {
                    course.Lessons = new List<Lesson>();
                }

                course.Lessons.Add(lesson);
                await _context.SaveChangesAsync();
                var result = await _context.Lessons.AsNoTracking().FirstOrDefaultAsync(c => c.Id == course.Id);
                return result;
            }
            return null;
        }*/

        public async Task<Lesson> UpdateLessonAsync(Lesson lesson)
        {
            var result = await _context.Lessons.FirstOrDefaultAsync(l => l.Id == lesson.Id);

            result.Time = lesson.Time;
            result.Course = lesson.Course;

            await _context.SaveChangesAsync();

            return result;

        }
        public async Task DeleteLessonAsync(int lessonId)
        {
            var result = await _context.Lessons.FirstOrDefaultAsync(l => l.Id == lessonId);
            _context.Lessons.Remove(result);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Lesson>> GetCourseLessonsAsync(int courseId)
        {
            return await _context.Lessons.AsNoTracking().Include(l => l.Course).Where(l => l.Course.Id == courseId).ToListAsync();
        }

        public async Task<IEnumerable<Lesson>> CreateLessonsForCourseAsync(Lesson[] lessons)
        {
            var course = await _context.Courses.Include(c => c.Lessons).FirstOrDefaultAsync(c => c.Id == lessons[0].Course.Id);

            if (course.Lessons is null)
            {
                course.Lessons = new List<Lesson>();
            }

            foreach (var lesson in lessons)
            {
                course.Lessons.Add(lesson);
            }

            await _context.SaveChangesAsync();

            var newTimes = course.Lessons.Select(l => l.Time).Intersect(lessons.Select(n => n.Time));
            return course.Lessons.Where(l => newTimes.Contains(l.Time));
        }

        public async Task<IEnumerable<Lesson>> GetStudentLessonsAsync(int studentId)
        {
            var student = await _context.Students.AsNoTracking().Include(s => s.Courses).ThenInclude(c => c.Lessons).FirstOrDefaultAsync(s => s.Id == studentId);
            var lessons = new List<Lesson>();
            foreach (var course in student.Courses)
            {
                lessons.AddRange(course.Lessons);
            }
            return lessons;
        }
    }
}
