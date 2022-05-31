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
        public async Task<ActionResult<LessonDto>> AddLessonsAsync(CreateLessonShortDto[] lessonsDto,int courseId)
        {
            Lesson[] lessons = new Lesson[lessonsDto.Length];

                for(int i = 0; i < lessonsDto.Length; i++)
            {
                TimeOnly lessonTime = LessonTimes[lessonsDto[i].lessonOfTheDay - 1];

                Lesson lesson = new();
                lesson.Course = new() { Id = courseId };
                lesson.Time = new DateTime(lessonsDto[i].day.Year, lessonsDto[i].day.Month, lessonsDto[i].day.Day, lessonTime.Hour, lessonTime.Minute, 0);
                lessons[i] = lesson;
            }

            await _repository.CreateLessonsForCourseAsync(lessons);
            return Ok();
            //return CreatedAtAction(nameof(GetStudentAsync), new { id = student.Id }, student.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<LessonDto>> UpdateLessonAsync(int id, UpdateLessonDto lessonDto)
        {
            var existingLesson = await _repository.GetLessonAsync(id);

            if(existingLesson is null)
            {
                return NotFound();
            }

            TimeOnly lessonTime = LessonTimes[lessonDto.lessonOfTheDay - 1];

            existingLesson.Course = new() { Id = lessonDto.courseId};
            existingLesson.Time = new DateTime(lessonDto.day.Year, lessonDto.day.Month, lessonDto.day.Day, lessonTime.Hour, lessonTime.Minute, 0);

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

    }
}
