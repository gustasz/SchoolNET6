using Microsoft.AspNetCore.Mvc;
using SchoolAPI.Data;
using SchoolAPI.Models;

namespace SchoolAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        private readonly ILessonRepository _repository;
        public static readonly TimeOnly[] LessonTimes = { // All lessons in school can only start on 8 different times, 1st lesson starts at [0]. For example, 5th lesson of the day can only start at 12:00
            new TimeOnly(8,0), new TimeOnly(8,55),new TimeOnly(9,50),new TimeOnly(10,55),new TimeOnly(12,0),new TimeOnly(12,55),new TimeOnly(13,50),new TimeOnly(14,45)};
        public LessonsController(ILessonRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IEnumerable<LessonDto>> GetLessonsAsync()
        {
            var lessons = (await _repository.GetLessonsAsync())
                            .Select(lesson => lesson.AsDto());
            return lessons;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LessonDto>> GetLessonAsync(int id)
        {
            var lesson = await _repository.GetLessonAsync(id);

            if (lesson is null)
            {
                return NotFound();
            }

            return lesson.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<LessonDto>> AddLessonsAsync(CreateLessonShortDto[] lessonsDto, int courseId)
        {
            Lesson[] lessons = new Lesson[lessonsDto.Length];

            for (int i = 0; i < lessonsDto.Length; i++)
            {
                TimeOnly lessonTime = LessonTimes[lessonsDto[i].LessonOfTheDay - 1];

                Lesson lesson = new();
                lesson.Course = new() { Id = courseId };
                lesson.Time = new DateTime(lessonsDto[i].DayDate.Year, lessonsDto[i].DayDate.Month, lessonsDto[i].DayDate.Day, lessonTime.Hour, lessonTime.Minute, 0);
                lessons[i] = lesson;
            }

            var currentLessons = await _repository.GetCourseLessonsAsync(courseId);

            var newLessonTimes = lessons.Select(l => l.Time);
            var currentLessonTimes = currentLessons.Select(l => l.Time);

            if(currentLessonTimes.Any(l => newLessonTimes.Contains(l)))
            {
                return BadRequest();
            }

            var result = await _repository.CreateLessonsForCourseAsync(lessons);

            return CreatedAtAction("GetLessons", null, result.ToList().AsDtos());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<LessonDto>> UpdateLessonAsync(int id, UpdateLessonDto lessonDto)
        {
            var existingLesson = await _repository.GetLessonAsync(id);

            if (existingLesson is null)
            {
                return NotFound();
            }

            TimeOnly lessonTime = LessonTimes[lessonDto.LessonOfTheDay - 1];

            existingLesson.Course = new() { Id = lessonDto.CourseId };
            existingLesson.Time = new DateTime(lessonDto.DayDate.Year, lessonDto.DayDate.Month, lessonDto.DayDate.Day, lessonTime.Hour, lessonTime.Minute, 0);

            await _repository.UpdateLessonAsync(existingLesson);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLessonAsync(int id)
        {
            var existingLesson = await _repository.GetLessonAsync(id);

            if (existingLesson is null)
            {
                return NotFound();
            }

            await _repository.DeleteLessonAsync(id);

            return NoContent();
        }

        [HttpGet("/course/{courseId}")]
        public async Task<IEnumerable<LessonDto>> GetCourseLessonsAsync(int courseId)
        {
            var lessons = (await _repository.GetCourseLessonsAsync(courseId))
                .Select(lesson => lesson.AsDto());
            return lessons;
        }

        [HttpGet("/student/{studentId}")]
        public async Task<IEnumerable<LessonDto>> GetStudentLessonsAsync(int studentId)
        {
            var lessons = (await _repository.GetStudentLessonsAsync(studentId))
                .Select(lesson => lesson.AsDto());
            return lessons;
        }

    }
}
