using Microsoft.AspNetCore.Mvc;
using SchoolAPI.Data;
using SchoolAPI.Models;

namespace SchoolAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherRepository _repository;
        public TeachersController(ITeacherRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IEnumerable<TeacherDto>> GetTeachersAsync()
        {
            var teachers = (await _repository.GetTeachersAsync())
                            .Select(teacher => teacher.AsDto());
            return teachers;

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherDto>> GetTeacherAsync(int id)
        {
            var teacher = await _repository.GetTeacherAsync(id);

            if (teacher is null)
            {
                return NotFound();
            }

            return teacher.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<TeacherDto>> CreateTeacherAsync(CreateTeacherDto teacherDto)
        {
            Teacher teacher = new()
            {
                //Id = 
                FirstName = teacherDto.FirstName,
                LastName = teacherDto.LastName,
                BirthDate = teacherDto.BirthDate
            };

            var result = await _repository.AddTeacherAsync(teacher);
            
            return CreatedAtAction("GetTeacher", new { id = result.Id }, result.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TeacherDto>> UpdateTeacherAsync(int id, UpdateTeacherDto teacherDto)
        {
            var existingTeacher = await _repository.GetTeacherAsync(id);

            if (existingTeacher is null)
            {
                return NotFound();
            }

            existingTeacher.FirstName = teacherDto.FirstName;
            existingTeacher.LastName = teacherDto.LastName;
            existingTeacher.BirthDate = teacherDto.BirthDate;

            await _repository.UpdateTeacherAsync(existingTeacher);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTeacherAsync(int id)
        {
            var existingTeacher = await _repository.GetTeacherAsync(id);

            if (existingTeacher is null)
            {
                return NotFound();
            }

            await _repository.DeleteTeacherAsync(id);

            return NoContent();
        }
    }
}
