using Microsoft.AspNetCore.Mvc;
using SchoolAPI.Data;
using SchoolAPI.Data.Interfaces;
using SchoolAPI.Models;

namespace SchoolAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LessonsController> _logger;

        public static readonly TimeOnly[] LessonTimes = { // Lessons can only start at predetermined times by school. For example, 5th lesson of the day can only start at 12:00
            new TimeOnly(8,0), new TimeOnly(8,55),new TimeOnly(9,50),new TimeOnly(10,55),new TimeOnly(12,0),new TimeOnly(12,55),new TimeOnly(13,50),new TimeOnly(14,45)};
        public LessonsController(IUnitOfWork unitOfWork, ILogger<LessonsController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<LessonDto>> GetLessonsAsync()
        {
            var lessons = (await _unitOfWork.Lesson.GetAllAsync())
                            .Select(lesson => lesson.AsDto());
            return lessons;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LessonDto>> GetLessonAsync(int id)
        {
            var lesson = await _unitOfWork.Lesson.GetByIdAsync(id);

            if (lesson is null)
            {
                _logger.LogWarning("Lesson with id:{Id} Not Found", id);
                return NotFound();
            }

            return lesson.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<LessonDto>> AddLessonsAsync(CreateLessonShortDto[] lessonsDto, int courseId)
        {
            var course = await _unitOfWork.Course.GetByIdAsync(courseId);
            if (course is null)
            {
                _logger.LogWarning("Course with id:{Id} Not Found", courseId);
                return NotFound();
            }

            Lesson[] lessons = new Lesson[lessonsDto.Length];
            for (int i = 0; i < lessonsDto.Length; i++)
            {
                TimeOnly lessonTime = LessonTimes[lessonsDto[i].LessonOfTheDay - 1];

                Lesson lesson = new();
                lesson.Course = course;
                lesson.Time = new DateTime(lessonsDto[i].DayDate.Year, lessonsDto[i].DayDate.Month, lessonsDto[i].DayDate.Day, lessonTime.Hour, lessonTime.Minute, 0);
                lessons[i] = lesson;
            }

            var newLessonTimes = lessons.Select(l => l.Time);

            var weekendLessons = newLessonTimes.Where(l => l.DayOfWeek == DayOfWeek.Saturday || l.DayOfWeek == DayOfWeek.Sunday);
            if (weekendLessons.Any())
            {
                string weekendString = String.Join(", ", weekendLessons);
                return BadRequest($"Trying to include lesson(s) on a weekend: {weekendString}");
            }

            var currentLessons = await _unitOfWork.Lesson.GetCourseLessonsAsync(courseId);
            var currentLessonTimes = currentLessons.Select(l => l.Time);
            var lessonOverlap = currentLessonTimes.Where(l => newLessonTimes.Contains(l)).ToList();
            if (lessonOverlap.Any())
            {
                string overlapS = String.Join(", ", lessonOverlap);
                return BadRequest($"Lesson(s) already exists: {overlapS}");
            }

            // check for student schedule overlap (there must be a better way to do this)
            var courseStudents = await _unitOfWork.Course.GetCourseStudentsAsync(courseId);
            var allStudentLessonTimes = new List<DateTime>();
            foreach (var student in courseStudents)
            {
                var studentLessons = await _unitOfWork.Lesson.GetStudentLessonsAsync(student.Id);
                foreach (var lesson in studentLessons)
                {
                    if (!allStudentLessonTimes.Contains(lesson.Time))
                    {
                        allStudentLessonTimes.Add(lesson.Time);
                    }
                }
            }
            var timeOverlapS = allStudentLessonTimes.Intersect(newLessonTimes);
            if (timeOverlapS.Any())
            {
                string oLessons = String.Join(", ", timeOverlapS);
                return BadRequest($"Schedule overlap for students: {oLessons}"); // return course too?
            }

            // check for teacher schedule overlap (there must be a better way to do this)
            var allTeacherLessonTimes = new List<DateTime>();
            var teacherLessons = await _unitOfWork.Lesson.GetTeacherLessonsAsync(course.Teacher.Id);
            foreach (var lesson in teacherLessons)
            {
                allTeacherLessonTimes.Add(lesson.Time);
            }
            var timeOverlapT = allTeacherLessonTimes.Intersect(newLessonTimes);
            if (timeOverlapT.Any())
            {
                string oLessons = String.Join(", ", timeOverlapT);
                return BadRequest($"Schedule overlap for teacher: {oLessons}"); // return course too?
            }

            var result = await _unitOfWork.Lesson.CreateLessonsForCourseAsync(lessons);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction("GetLessons", null, result.ToList().AsDtos());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<LessonDto>> UpdateLessonAsync(int id, UpdateLessonDto lessonDto)
        {
            var existingLesson = await _unitOfWork.Lesson.GetByIdAsync(id);

            if (existingLesson is null)
            {
                _logger.LogWarning("Lesson with id:{Id} Not Found", id);
                return NotFound();
            }

            var existingCourse = await _unitOfWork.Course.GetByIdAsync(lessonDto.CourseId);
            if (existingCourse is null)
            {
                _logger.LogWarning("Course with id:{Id} Not Found", lessonDto.CourseId);
                return NotFound();
            }

            if (lessonDto.DayDate.DayOfWeek == DayOfWeek.Saturday || lessonDto.DayDate.DayOfWeek == DayOfWeek.Sunday)
            {
                return BadRequest($"Trying to include a lesson that is on a weekend: {lessonDto.DayDate}");
            }

            TimeOnly lessonTime = LessonTimes[lessonDto.LessonOfTheDay - 1];

            existingLesson.Course = new() { Id = lessonDto.CourseId };
            existingLesson.Time = new DateTime(lessonDto.DayDate.Year, lessonDto.DayDate.Month, lessonDto.DayDate.Day, lessonTime.Hour, lessonTime.Minute, 0);

            var currentLessons = await _unitOfWork.Lesson.GetCourseLessonsAsync(lessonDto.CourseId);
            var currentLessonTimes = currentLessons.Select(l => l.Time);
            if (currentLessonTimes.Any(l => l == existingLesson.Time))
            {
                return BadRequest($"Lesson at that time already exists: {existingLesson.Time}");
            }

            // check for student schedule overlap (there must be a better way to do this)
            var courseStudents = await _unitOfWork.Course.GetCourseStudentsAsync(lessonDto.CourseId);
            var allStudentLessonTimes = new List<DateTime>();
            foreach (var student in courseStudents)
            {
                var studentLessons = await _unitOfWork.Lesson.GetStudentLessonsAsync(student.Id);
                foreach (var lesson in studentLessons)
                {
                    if (!allStudentLessonTimes.Contains(lesson.Time))
                    {
                        allStudentLessonTimes.Add(lesson.Time);
                    }
                }
            }
            if (allStudentLessonTimes.Contains(existingLesson.Time))
            {
                return BadRequest($"Schedule overlap for students: {existingLesson.Time}"); // return course too?
            }

            // check for teacher schedule overlap (there must be a better way to do this)
            var allTeacherLessonTimes = new List<DateTime>();
            var teacherLessons = await _unitOfWork.Lesson.GetTeacherLessonsAsync(existingCourse.Teacher.Id);
            foreach (var lesson in teacherLessons)
            {
                allTeacherLessonTimes.Add(lesson.Time);
            }
            if (allTeacherLessonTimes.Contains(existingLesson.Time))
            {
                return BadRequest($"Schedule overlap for teacher: {existingLesson.Time}"); // return course too?
            }

            await _unitOfWork.Lesson.UpdateAsync(existingLesson);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLessonAsync(int id)
        {
            var existingLesson = await _unitOfWork.Lesson.GetByIdAsync(id);

            if (existingLesson is null)
            {
                _logger.LogWarning("Lesson with id:{Id} Not Found", id);
                return NotFound();
            }

            await _unitOfWork.Lesson.RemoveAsync(id);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpGet("course/{courseId}")]
        public async Task<IEnumerable<LessonDto>> GetCourseLessonsAsync(int courseId)
        {
            var lessons = (await _unitOfWork.Lesson.GetCourseLessonsAsync(courseId))
                .Select(lesson => lesson.AsDto());
            return lessons;
        }

        [HttpGet("student/{studentId}")]
        public async Task<IEnumerable<LessonDto>> GetStudentLessonsAsync(int studentId)
        {
            var lessons = (await _unitOfWork.Lesson.GetStudentLessonsAsync(studentId))
                .Select(lesson => lesson.AsDto());
            return lessons;
        }

        [HttpGet("student/{studentId}/day")]
        public async Task<IEnumerable<LessonDto>> GetStudentLessonsForDayAsync(int studentId, DateTime dayDate)
        {
            var lessons = (await _unitOfWork.Lesson.GetStudentLessonsAsync(studentId));
            var dayLessons = lessons.Where(l => l.Time.Date == dayDate.Date).OrderBy(l => l.Time);
            return dayLessons.Select(lesson => lesson.AsDto());
        }

        [HttpGet("teacher/{teacherId}")]
        public async Task<IEnumerable<LessonDto>> GetTeacherLessonsAsync(int teacherId)
        {
            var lessons = (await _unitOfWork.Lesson.GetTeacherLessonsAsync(teacherId))
                .Select(lesson => lesson.AsDto());
            return lessons;
        }

        [HttpGet("teacher/{teacherId}/day")]
        public async Task<IEnumerable<LessonDto>> GetTeacherLessonsForDayAsync(int teacherId, DateTime dayDate)
        {
            var lessons = (await _unitOfWork.Lesson.GetTeacherLessonsAsync(teacherId));
            var dayLessons = lessons.Where(l => l.Time.Date == dayDate.Date).OrderBy(l => l.Time);
            return dayLessons.Select(lesson => lesson.AsDto());
        }

    }
}
