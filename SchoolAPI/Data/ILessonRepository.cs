using SchoolAPI.Models;

namespace SchoolAPI.Data
{
    public interface ILessonRepository
    {
        Task<IEnumerable<Lesson>> GetLessonsAsync();
        Task<Lesson> GetLessonAsync(int lessonId);
        Task<Lesson> AddLessonAsync(Lesson lesson);
        Task<Lesson> UpdateLessonAsync(Lesson lesson);
        Task DeleteLessonAsync(int lessonId);
        Task<IEnumerable<Lesson>> GetCourseLessonsAsync(int courseId);
        Task<Course> CreateLessonsForCourseAsync(Lesson[] lessons);

        // get lessons for student for certain day
        // get all lessons for a course
    }
}
