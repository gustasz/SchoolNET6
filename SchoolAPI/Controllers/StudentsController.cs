using Microsoft.AspNetCore.Http;
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
        public StudentsController(IStudentRepository repository)
        {
            _repository = repository;
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
                return NotFound();
            }

            return student.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<StudentDto>> CreateStudentAsync(CreateStudentDto studentDto)
        {
            Student student = new()
            {
                //Id = 
                FirstName = studentDto.FirstName,
                LastName = studentDto.LastName,
                BirthDate = studentDto.BirthDate,
                Grade = studentDto.Grade
            };

            await _repository.AddStudentAsync(student);
            return Ok();
            //return CreatedAtAction(nameof(GetStudentAsync), new { id = student.Id }, student.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<StudentDto>> UpdateStudentAsync(int id, UpdateStudentDto studentDto)
        {
            var existingStudent = await _repository.GetStudentAsync(id);

            if(existingStudent is null)
            {
                return NotFound();
            }

            existingStudent.FirstName = studentDto.FirstName;
            existingStudent.LastName = studentDto.LastName;
            existingStudent.BirthDate = studentDto.BirthDate;
            existingStudent.Grade = studentDto.Grade;

            await _repository.UpdateStudentAsync(existingStudent);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStudentAsync(int id)
        {
            var existingStudent = await _repository.GetStudentAsync(id);

            if(existingStudent is null)
            {
                return NotFound();
            }

            await _repository.DeleteStudentAsync(id);

            return NoContent();
        }
    }
}
