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

            course.Subject = new() { Id = courseDto.subjectId };
            course.Teacher = new() { Id = courseDto.teacherId };

            await _repository.AddCourseAsync(course);
            return Ok();
            //return CreatedAtAction(nameof(GetStudentAsync), new { id = student.Id }, student.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CourseDto>> UpdateCourseAsync(int id, UpdateCourseDto courseDto)
        {
            var existingCourse = await _repository.GetCourseAsync(id);

            if (existingCourse is null)
            {
                return NotFound();
            }

            existingCourse.Subject = new() { Id = courseDto.subjectId };
            existingCourse.Teacher = new() { Id = courseDto.teacherId };

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
    }
}
