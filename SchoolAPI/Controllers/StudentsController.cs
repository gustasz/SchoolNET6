using Microsoft.AspNetCore.Mvc;
using SchoolAPI.Data;
using SchoolAPI.Models;

namespace SchoolAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository _repository;
        private readonly ICourseRepository _courseRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly ILogger<StudentsController> _logger;
        public StudentsController(IStudentRepository repository, ICourseRepository courseRepository, ILessonRepository lessonRepository, ILogger<StudentsController> logger)
        {
            _repository = repository;
            _courseRepository = courseRepository;
            _lessonRepository = lessonRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<StudentDto>> GetStudentsAsync()
        {
            var students = (await _repository.GetStudentsAsync())
                            .Select(student => student.AsDto());
            return students;

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetStudentAsync(int id)
        {
            var student = await _repository.GetStudentAsync(id);

            if (student is null)
            {
                _logger.LogWarning("Student with id:{Id} Not Found", id);
                return NotFound();
            }

            return student.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<StudentDto>> CreateStudentAsync(CreateStudentDto studentDto)
        {
            Student student = new()
            {
                FirstName = studentDto.FirstName,
                LastName = studentDto.LastName,
                BirthDate = studentDto.BirthDate,
                Grade = studentDto.Grade,
                Class = studentDto.Class
            };

            student.BirthDate = new DateTime(student.BirthDate.Year, student.BirthDate.Month, student.BirthDate.Day);

            var result = await _repository.AddStudentAsync(student);

            return CreatedAtAction("GetStudent", new { id = result.Id }, result.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<StudentDto>> UpdateStudentAsync(int id, UpdateStudentDto studentDto)
        {
            var existingStudent = await _repository.GetStudentAsync(id);

            if (existingStudent is null)
            {
                _logger.LogWarning("Student with id:{Id} Not Found", id);
                return NotFound();
            }

            existingStudent.FirstName = studentDto.FirstName;
            existingStudent.LastName = studentDto.LastName;
            existingStudent.BirthDate = studentDto.BirthDate;
            existingStudent.Grade = studentDto.Grade;
            existingStudent.Class = studentDto.Class;

            await _repository.UpdateStudentAsync(existingStudent);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStudentAsync(int id)
        {
            var existingStudent = await _repository.GetStudentAsync(id);

            if (existingStudent is null)
            {
                _logger.LogWarning("Student with id:{Id} Not Found", id);
                return NotFound();
            }

            await _repository.DeleteStudentAsync(id);

            return NoContent();
        }

        //GET /students/<student-id>/courses
        [HttpGet("{studentId}/courses")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetStudentCoursesAsync(int studentId)
        {
            var student = await _repository.GetStudentAsync(studentId);
            if (student is null)
            {
                _logger.LogWarning("Student with id:{Id} Not Found", studentId);
                return NotFound();
            }

            var courses = (await _repository.GetStudentCoursesAsync(studentId))
                            .Select(course => course.AsDto());
            return Ok(courses);
        }

        //PUT /students/<student-id>/courses/<course-id>
        [HttpPut("{studentId}/courses/{courseId}")]
        public async Task<ActionResult<StudentDto>> AddCourseToStudentAsync(int studentId, int courseId)
        {
            var student = await _repository.GetStudentAsync(studentId);
            if (student is null)
            {
                _logger.LogWarning("Student with id:{Id} Not Found", studentId);
                return NotFound();
            }

            var course = await _courseRepository.GetCourseAsync(courseId);
            if (course is null)
            {
                _logger.LogWarning("Course with id:{Id} Not Found", courseId);
                return NotFound();
            }

            if (course.ForGrade != student.Grade)
            {
                return BadRequest($"Trying to add a student that is in {student.Grade} grade into a {course.ForGrade} grade course.");
            }

            if (course.ForClass != 0 && course.ForClass != student.Class)
            {
                return BadRequest($"Trying to add a student in {student.Class} class into a {course.ForClass} only course.");
            }

            var courseStudents = await _courseRepository.GetCourseStudentsAsync(courseId);
            if (courseStudents.Any(s => s.Id == studentId))
            {
                return BadRequest("Course already has the student assigned.");
            }

            // check for schedule overlap
            var studentLessonsTimes = (await _lessonRepository.GetStudentLessonsAsync(studentId)).Select(l => l.Time);
            var courseLessonsTimes = (await _lessonRepository.GetCourseLessonsAsync(courseId)).Select(l => l.Time);

            var overlapTimes = studentLessonsTimes.Intersect(courseLessonsTimes);
            if (overlapTimes.Any())
            {
                string overlapStrings = String.Join(", ", overlapTimes);
                return BadRequest($"Schedule overlap: {overlapStrings}");
            }

            await _repository.AddCourseToStudentAsync(studentId, courseId);

            return NoContent();
        }

        //DELETE /students/<student-id>/courses/<course-id>
        [HttpDelete("{studentId}/courses/{courseId}")]
        public async Task<ActionResult> DeleteStudentFromCourseAsync(int studentId, int courseId)
        {
            var student = await _repository.GetStudentAsync(studentId);
            if (student is null)
            {
                _logger.LogWarning("Student with id:{Id} Not Found", studentId);
                return NotFound();
            }

            var course = await _courseRepository.GetCourseAsync(courseId);
            if (course is null)
            {
                _logger.LogWarning("Course with id:{Id} Not Found", courseId);
                return NotFound();
            }

            if (!student.Courses.Contains(course))
            {
                _logger.LogWarning("Course with id:{CourseId} Not Found in Student with id:{StudentId} courses", courseId, studentId);
                return NotFound();
            }

            await _repository.DeleteCourseFromStudentAsync(studentId, courseId);

            return NoContent();
        }

        //GET /students/grade/<grade-id>/class/<class-id>
        [HttpGet("grade/{gradeId}/class/{classId}")]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudentsFromClassAsync(int gradeId, int classId)
        {
            var students = (await _repository.GetStudentsFromClassAsync(gradeId,classId))
                            .Select(student => student.AsDto());
            return Ok(students);
        }
    }
}
