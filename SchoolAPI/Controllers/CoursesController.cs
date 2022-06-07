using Microsoft.AspNetCore.Mvc;
using SchoolAPI.Data;
using SchoolAPI.Models;

namespace SchoolAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseRepository _repository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ILessonRepository _lessonRepository;
        public CoursesController(ICourseRepository repository,
                                 ISubjectRepository subjectRepository,
                                 ITeacherRepository teacherRepository,
                                 IStudentRepository studentRepository,
                                 ILessonRepository lessonRepository)
        {
            _repository = repository;
            _subjectRepository = subjectRepository;
            _teacherRepository = teacherRepository;
            _studentRepository = studentRepository;
            _lessonRepository = lessonRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<CourseDto>> GetCoursesAsync()
        {
            var courses = (await _repository.GetCoursesAsync())
                            .Select(course => course.AsDto());
            return courses;

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCourseAsync(int id)
        {
            var course = await _repository.GetCourseAsync(id);

            if (course is null)
            {
                return NotFound();
            }

            return course.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<CourseDto>> CreateCourseAsync(CreateCourseDto courseDto)
        {
            var subject = await _subjectRepository.GetSubjectAsync(courseDto.SubjectId);
            if (subject is null)
            {
                return NotFound("Subject not found.");
            }

            var teacher = await _teacherRepository.GetTeacherAsync(courseDto.TeacherId);
            if (teacher is null)
            {
                return NotFound("Teacher not found.");
            }

            var currentCourses = await GetCoursesAsync();
            if (currentCourses.Any((c => c.SubjectId == courseDto.SubjectId && c.TeacherId == courseDto.TeacherId)))
            {
                return BadRequest("Course already exists");
            }

            Course course = new();

            course.Subject = subject;
            course.Teacher = teacher;

            var result = await _repository.AddCourseAsync(course);

            return CreatedAtAction("GetCourse", new { id = result.Id }, result.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CourseDto>> UpdateCourseAsync(int id, UpdateCourseDto courseDto)
        {
            var existingCourse = await _repository.GetCourseAsync(id);
            if (existingCourse is null)
            {
                return NotFound();
            }

            var subject = await _subjectRepository.GetSubjectAsync(courseDto.SubjectId);
            if (subject is null)
            {
                return NotFound();
            }

            var teacher = await _teacherRepository.GetTeacherAsync(courseDto.TeacherId);
            if (teacher is null)
            {
                return NotFound();
            }

            existingCourse.Subject = subject;
            existingCourse.Teacher = teacher;

            await _repository.UpdateCourseAsync(existingCourse);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCourseAsync(int id)
        {
            var course = await _repository.GetCourseAsync(id);

            if (course is null)
            {
                return NotFound();
            }

            await _repository.DeleteCourseAsync(id);

            return NoContent();
        }

        //GET /courses/<course-id>/students
        [HttpGet("{courseId}/students")]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetCourseStudentsAsync(int courseId)
        {
            var course = await _repository.GetCourseAsync(courseId);
            if (course is null)
            {
                return NotFound();
            }

            var students = (await _repository.GetCourseStudentsAsync(courseId))
                                    .Select(student => student.AsDto());

            return Ok(students);
        }

        //PUT /courses/<course-id>/students/<student-id>
        [HttpPut("{courseId}/students/{studentId}")]
        public async Task<ActionResult<CourseDto>> AddStudentToCourse(int courseId, int studentId)
        {
            var course = await _repository.GetCourseAsync(courseId);
            if (course is null)
            {
                return NotFound();
            }

            var student = await _studentRepository.GetStudentAsync(studentId);
            if (student is null)
            {
                return NotFound();
            }
            

            var courseStudents = await _repository.GetCourseStudentsAsync(courseId);
            if (courseStudents.Any(s => s.Id == studentId))
            {
                return BadRequest("Student is already assigned to the course.");
            }

            var studentLessonsTimes = (await _lessonRepository.GetStudentLessonsAsync(studentId)).Select(l => l.Time);
            var courseLessonsTimes = (await _lessonRepository.GetCourseLessonsAsync(courseId)).Select(l => l.Time);
            if(studentLessonsTimes.Intersect(courseLessonsTimes).Any())
            {
                return BadRequest("Schedule overlap."); // return students and dates?
            }

            await _repository.AddStudentToCourseAsync(courseId, studentId);

            return NoContent();
        }

        //DELETE /courses/<course-id>/students/<student-id>
        [HttpDelete("{courseId}/students/{studentId}")]
        public async Task<ActionResult> DeleteStudentFromCourse(int courseId, int studentId)
        {
            var course = await _repository.GetCourseAsync(courseId);
            if (course is null)
            {
                return NotFound();
            }

            var student = await _studentRepository.GetStudentAsync(studentId);
            if (student is null)
            {
                return NotFound();
            }

            var courseStudents = await _repository.GetCourseStudentsAsync(courseId);
            if (!courseStudents.Any(s => s.Id == studentId))
            {
                return NotFound();
            }

            await _repository.DeleteStudentFromCourseAsync(courseId, studentId);

            return NoContent();
        }

        //PUT /courses/coursetimes
        [HttpPut("coursetimes")]
        public async Task<ActionResult<CourseDto>> AddCourseTimeToCourse(int courseId, int studentId)
        {
            var course = await _repository.GetCourseAsync(courseId);

            if (course is null)
            {
                return NotFound();
            }

            await _repository.AddStudentToCourseAsync(courseId, studentId);

            return NoContent();
        }
    }
}
