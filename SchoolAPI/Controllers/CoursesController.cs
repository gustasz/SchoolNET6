using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SchoolAPI.Data;
using SchoolAPI.Data.Interfaces;
using SchoolAPI.Models;

namespace SchoolAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CoursesController> _logger;
        public CoursesController(IUnitOfWork unitOfWork,
                                 ILogger<CoursesController> logger,
                                 IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<IEnumerable<CourseDto>> GetCoursesAsync()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.Courses, out IEnumerable<CourseDto> courses))
            {
                courses = (await _unitOfWork.Course.GetAllAsync())
                            .Select(course => course.AsDto());

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));

                _memoryCache.Set(CacheKeys.Courses, courses, cacheEntryOptions);
            }
            return courses;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCourseAsync(int id)
        {
            var course = await _unitOfWork.Course.GetByIdAsync(id);

            if (course is null)
            {
                _logger.LogWarning("Course with id:{Id} Not Found", id);
                return NotFound();
            }

            return course.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<CourseDto>> CreateCourseAsync(CreateCourseDto courseDto)
        {
            var subject = await _unitOfWork.Subject.GetByIdAsync(courseDto.SubjectId);
            if (subject is null)
            {
                _logger.LogWarning("Subject with id:{Id} Not Found", courseDto.SubjectId);
                return NotFound("Subject not found.");
            }

            var teacher = await _unitOfWork.Teacher.GetByIdAsync(courseDto.TeacherId);
            if (teacher is null)
            {
                _logger.LogWarning("Teacher with id:{Id} Not Found", courseDto.TeacherId);
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

            var result = await _unitOfWork.Course.AddAsync(course);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction("GetCourse", new { id = result.Id }, result.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CourseDto>> UpdateCourseAsync(int id, UpdateCourseDto courseDto)
        {
            var existingCourse = await _unitOfWork.Course.GetByIdAsync(id);
            if (existingCourse is null)
            {
                _logger.LogWarning("Course with id:{Id} Not Found", id);
                return NotFound();
            }

            var subject = await _unitOfWork.Subject.GetByIdAsync(courseDto.SubjectId);
            if (subject is null)
            {
                _logger.LogWarning("Subject with id:{Id} Not Found", courseDto.SubjectId);
                return NotFound();
            }

            var teacher = await _unitOfWork.Teacher.GetByIdAsync(courseDto.TeacherId);
            if (teacher is null)
            {
                _logger.LogWarning("Teacher with id:{Id} Not Found", courseDto.TeacherId);
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

            await _unitOfWork.Course.UpdateAsync(existingCourse);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCourseAsync(int id)
        {
            var course = await _unitOfWork.Course.GetByIdAsync(id);

            if (course is null)
            {
                _logger.LogWarning("Course with id:{Id} Not Found", id);
                return NotFound();
            }

            await _unitOfWork.Course.RemoveAsync(id);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        //GET /courses/<course-id>/students
        [HttpGet("{courseId}/students")]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetCourseStudentsAsync(int courseId)
        {
            var course = await _unitOfWork.Course.GetByIdAsync(courseId);
            if (course is null)
            {
                _logger.LogWarning("Course with id:{Id} Not Found", courseId);
                return NotFound();
            }

            var students = (await _unitOfWork.Course.GetCourseStudentsAsync(courseId))
                                    .Select(student => student.AsDto());

            return Ok(students);
        }

        //PUT /courses/<course-id>/students/<student-id>
        [HttpPut("{courseId}/students/{studentId}")]
        public async Task<ActionResult<CourseDto>> AddStudentToCourse(int courseId, int studentId)
        {
            var course = await _unitOfWork.Course.GetByIdAsync(courseId);
            if (course is null)
            {
                _logger.LogWarning("Course with id:{Id} Not Found", courseId);
                return NotFound();
            }

            var student = await _unitOfWork.Student.GetByIdAsync(studentId);
            if (student is null)
            {
                _logger.LogWarning("Student with id:{Id} Not Found", studentId);
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

            var courseStudents = await _unitOfWork.Course.GetCourseStudentsAsync(courseId);
            if (courseStudents.Any(s => s.Id == studentId))
            {
                return BadRequest("Student is already assigned to the course.");
            }

            var studentLessonsTimes = (await _unitOfWork.Lesson.GetStudentLessonsAsync(studentId)).Select(l => l.Time);
            var courseLessonsTimes = (await _unitOfWork.Lesson.GetCourseLessonsAsync(courseId)).Select(l => l.Time);
            var intersectTimes = studentLessonsTimes.Intersect(courseLessonsTimes);
            if (intersectTimes.Any())
            {
                var overlapS = String.Join(", ", intersectTimes);
                return BadRequest($"Schedule overlap: {overlapS}");
            }

            await _unitOfWork.Course.AddStudentToCourseAsync(courseId, studentId);

            return NoContent();
        }

        //DELETE /courses/<course-id>/students/<student-id>
        [HttpDelete("{courseId}/students/{studentId}")]
        public async Task<ActionResult> DeleteStudentFromCourse(int courseId, int studentId)
        {
            var course = await _unitOfWork.Course.GetByIdAsync(courseId);
            if (course is null)
            {
                _logger.LogWarning("Course with id:{Id} Not Found", courseId);
                return NotFound();
            }

            var student = await _unitOfWork.Student.GetByIdAsync(studentId);
            if (student is null)
            {
                _logger.LogWarning("Student with id:{Id} Not Found", studentId);
                return NotFound();
            }

            var courseStudents = await _unitOfWork.Course.GetCourseStudentsAsync(courseId);
            if (!courseStudents.Any(s => s.Id == studentId))
            {
                _logger.LogWarning("Student with id:{StudentId} Not Found in Course with id:{CourseID} students", studentId, courseId);
                return NotFound();
            }

            await _unitOfWork.Course.DeleteStudentFromCourseAsync(courseId, studentId);

            return NoContent();
        }

       /* //PUT /courses/coursetimes
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
        }*/

        //PUT /courses/<course-id>/grade
        [HttpPut("{courseId}/grade")]
        public async Task<ActionResult<CourseDto>> AddClassToCourse(int courseId, int gradeNum, int classNum)
        {
            var course = await _unitOfWork.Course.GetByIdAsync(courseId);
            if (course is null)
            {
                _logger.LogWarning("Course with id:{Id} Not Found", courseId);
                return NotFound();
            }

            var students = await _unitOfWork.Student.GetStudentsFromClassAsync(gradeNum, classNum);
            if (!students.Any())
            {
                _logger.LogWarning("Students in grade:{GradeNum} and class:{ClassNum} Not Found", gradeNum, classNum);
                return NotFound();
            }

            if (course.ForGrade != gradeNum)
            {
                return BadRequest($"Trying to add students that are in {gradeNum} grade into a {course.ForGrade} grade course.");
            }

            if (course.ForClass != 0 && course.ForClass != classNum)
            {
                return BadRequest($"Trying to add students that are in {classNum} class into a {course.ForClass} only course.");
            }

            var courseStudents = await _unitOfWork.Course.GetCourseStudentsAsync(courseId);
            var intersectStudents = courseStudents.Intersect(students);
            if (intersectStudents.Any()) 
            {
                string intersectString = String.Join(", ", intersectStudents);
                return BadRequest($"There are students already assigned in the course: {intersectString}");
            }

            List<DateTime> allStudentLessonTimes = new();
            foreach(var student in students)
            {
                List<DateTime> studentLessonsTimes = (await _unitOfWork.Lesson.GetStudentLessonsAsync(student.Id)).Select(l => l.Time).ToList();
                allStudentLessonTimes = studentLessonsTimes.Union(allStudentLessonTimes).ToList();
            }
            var courseLessonsTimes = (await _unitOfWork.Lesson.GetCourseLessonsAsync(courseId)).Select(l => l.Time);
            var intersectTimes = allStudentLessonTimes.Intersect(courseLessonsTimes);
            if (intersectTimes.Any())
            {
                var overlapS = String.Join(", ", intersectTimes);
                return BadRequest($"Schedule overlap: {overlapS}");
            }

            await _unitOfWork.Student.AddStudentsToCourseAsync(students.ToArray(),courseId);

            return NoContent();
        }

        //DELETE /courses/<course-id>/grade
        [HttpDelete("{courseId}/grade")]
        public async Task<ActionResult> DeleteClassFromCourse(int courseId, int gradeNum, int classNum)
        {
            var course = await _unitOfWork.Course.GetByIdAsync(courseId);
            if (course is null)
            {
                _logger.LogWarning("Course with id:{Id} Not Found", courseId);
                return NotFound();
            }

            var students = await _unitOfWork.Student.GetStudentsFromClassAsync(gradeNum, classNum);
            if (!students.Any())
            {
                _logger.LogWarning("Students in grade:{GradeNum} and class:{ClassNum} Not Found", gradeNum, classNum);
                return NotFound();
            }

            if (course.ForGrade != gradeNum)
            {
                return BadRequest($"Trying to remove students that are in {gradeNum} grade from a {course.ForGrade} grade course.");
            }

            if (course.ForClass != 0 && course.ForClass != classNum)
            {
                return BadRequest($"Trying to remove students that are in {classNum} class from a {course.ForClass} only course.");
            }

            var courseStudentsIds = (await _unitOfWork.Course.GetCourseStudentsAsync(courseId)).Select(s => s.Id);
            var studentIds = students.Select(s => s.Id);
            if (!courseStudentsIds.Intersect(studentIds).Any())
            {
                _logger.LogWarning("Students in grade:{GradeNum} and class:{ClassNum} Not Found in Course with id:{CourseId} students", gradeNum, classNum, courseId);
                return NotFound();
            }

            await _unitOfWork.Course.DeleteClassFromCourseAsync(courseId, gradeNum, classNum);

            return NoContent();
        }
    }
}
