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
        public CoursesController(ICourseRepository repository)
        {
            _repository = repository;
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
            Course course = new();

            course.Subject = new() { Id = courseDto.SubjectId };
            course.Teacher = new() { Id = courseDto.TeacherId };

            var result = await _repository.AddCourseAsync(course);

            if (result is null)
            {
                return BadRequest();
            }

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

            existingCourse.Subject = new() { Id = courseDto.SubjectId };
            existingCourse.Teacher = new() { Id = courseDto.TeacherId };

            await _repository.UpdateCourseAsync(existingCourse);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCourseAsync(int id)
        {
            var existingCourse = await _repository.GetCourseAsync(id);

            if (existingCourse is null)
            {
                return NotFound();
            }

            await _repository.DeleteCourseAsync(id);

            return NoContent();
        }

        //GET /courses/<course-id>/students
        [HttpGet("{courseId}/students")]
        public async Task<IEnumerable<StudentDto>> GetCourseStudentsAsync(int courseId)
        {
            var students = (await _repository.GetCourseStudentsAsync(courseId))
                                    .Select(student => student.AsDto());

            return students;
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

            var courseStudents = await _repository.GetCourseStudentsAsync(courseId);
            if (courseStudents.Any(s => s.Id == studentId))
            {
                return BadRequest();
            }

            await _repository.AddStudentToCourse(courseId, studentId);

            return NoContent();
        }

        //DELETE /courses/<course-id>/students/<student-id>
        [HttpDelete("{courseId}/students/{studentId}")]
        public async Task<ActionResult> DeleteStudentFromCourse(int courseId, int studentId)
        {
            var existingCourse = await _repository.GetCourseAsync(courseId);
            if (existingCourse is null)
            {
                return NotFound();
            }

            var courseStudents = await _repository.GetCourseStudentsAsync(courseId);
            if (!courseStudents.Any(s => s.Id == studentId))
            {
                return NotFound();
            }

            await _repository.DeleteStudentFromCourse(courseId, studentId);

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

            await _repository.AddStudentToCourse(courseId, studentId);

            return NoContent();
        }
    }
}
