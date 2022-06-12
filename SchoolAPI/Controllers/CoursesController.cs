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
            if (currentCourses.Any((c => c.SubjectId == courseDto.SubjectId && c.TeacherId == courseDto.TeacherId && c.ForGrade == courseDto.ForGrade && c.ForClass == courseDto.ForClass)))
            {
                return BadRequest("Course already exists");
            }

            Course course = new();

            course.Subject = subject;
            course.Teacher = teacher;
            course.ForGrade = courseDto.ForGrade;
            course.ForClass = courseDto.ForClass;
            course.Students = new List<Student>();
            course.Lessons = new List<Lesson>();

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

            var currentCourses = await GetCoursesAsync();
            if (currentCourses.Any((c => c.SubjectId == courseDto.SubjectId && c.TeacherId == courseDto.TeacherId && c.ForGrade == courseDto.ForGrade && c.ForClass == courseDto.ForClass)))
            {
                return BadRequest("Course already exists");
            }

            existingCourse.Subject = subject;
            existingCourse.Teacher = teacher;
            existingCourse.ForGrade = courseDto.ForGrade;
            existingCourse.ForClass = courseDto.ForClass;

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

            if (course.ForGrade != student.Grade)
            {
                return BadRequest($"Trying to add a student that is in {student.Grade} grade into a {course.ForGrade} grade course.");
            }

            if (course.ForClass != 0 && course.ForClass != student.Class)
            {
                return BadRequest($"Trying to add a student in {student.Class} class into a {course.ForClass} only course.");
            }

            var courseStudents = await _repository.GetCourseStudentsAsync(courseId);
            if (courseStudents.Any(s => s.Id == studentId))
            {
                return BadRequest("Student is already assigned to the course.");
            }

            var studentLessonsTimes = (await _lessonRepository.GetStudentLessonsAsync(studentId)).Select(l => l.Time);
            var courseLessonsTimes = (await _lessonRepository.GetCourseLessonsAsync(courseId)).Select(l => l.Time);
            var intersectTimes = studentLessonsTimes.Intersect(courseLessonsTimes);
            if (intersectTimes.Any())
            {
                var overlapS = String.Join(", ", intersectTimes);
                return BadRequest($"Schedule overlap: {overlapS}");
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

        //PUT /courses/<course-id>/grade/<grade-id>/class/<class-id> // too long?
        [HttpPut("{courseId}/grade/{gradeId}/class/{classId}")]
        public async Task<ActionResult<CourseDto>> AddClassToCourse(int courseId, int gradeId, int classId)
        {
            var course = await _repository.GetCourseAsync(courseId);
            if (course is null)
            {
                return NotFound();
            }

            var students = await _studentRepository.GetStudentsFromClassAsync(gradeId, classId);
            if (students.First() is null)
            {
                return NotFound();
            }

            if (course.ForGrade != gradeId)
            {
                return BadRequest($"Trying to add students that are in {gradeId} grade into a {course.ForGrade} grade course.");
            }

            if (course.ForClass != 0 && course.ForClass != classId)
            {
                return BadRequest($"Trying to add students that are in {classId} class into a {course.ForClass} only course.");
            }

            /*var courseStudents = await _repository.GetCourseStudentsAsync(courseId);
            if (courseStudents.Any(s => s.Id == studentId))
            {
                return BadRequest("Student is already assigned to the course.");
            }*/
            //var studentLessonsTimes = new List<DateTime>();

            /*foreach(var student in students)
            {
                var studentLessonsTimes = (await _lessonRepository.GetStudentLessonsAsync(student.Id)).Select(l => l.Time);
            }
            var courseLessonsTimes = (await _lessonRepository.GetCourseLessonsAsync(courseId)).Select(l => l.Time);
            var intersectTimes = studentLessonsTimes.Intersect(courseLessonsTimes);
            if (intersectTimes.Any())
            {
                var overlapS = String.Join(", ", intersectTimes);
                return BadRequest($"Schedule overlap: {overlapS}");
            }*/

            await _studentRepository.AddStudentsToCourseAsync(students.ToArray(),courseId);

            return NoContent();
        }
    }
}
