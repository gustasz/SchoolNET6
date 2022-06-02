﻿using SchoolAPI.Models;

namespace SchoolAPI.Data
{
    public interface ILessonRepository
    {
        Task<IEnumerable<Lesson>> GetLessonsAsync();
        Task<Lesson> GetLessonAsync(int lessonId);
        Task<IEnumerable<Lesson>> CreateLessonsForCourseAsync(Lesson[] lessons);
        Task<Lesson> AddLessonAsync(Lesson lesson); // ?
        Task<Lesson> UpdateLessonAsync(Lesson lesson);
        Task DeleteLessonAsync(int lessonId);
        Task<IEnumerable<Lesson>> GetCourseLessonsAsync(int courseId);
        Task<IEnumerable<Lesson>> GetStudentLessonsAsync(int studentId);
        // get lessons for student for certain day
        // get all lessons for a course
    }
}
