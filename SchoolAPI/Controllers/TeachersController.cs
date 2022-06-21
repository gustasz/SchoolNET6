using Microsoft.AspNetCore.Mvc;
using SchoolAPI.Data;
using SchoolAPI.Data.Interfaces;
using SchoolAPI.Models;

namespace SchoolAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TeachersController> _logger;
        public TeachersController(IUnitOfWork unitOfWork, ILogger<TeachersController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<TeacherDto>> GetTeachersAsync()
        {
            var teachers = (await _unitOfWork.Teacher.GetAllAsync())
                            .Select(teacher => teacher.AsDto());
            return teachers;

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherDto>> GetTeacherAsync(int id)
        {
            var teacher = await _unitOfWork.Teacher.GetByIdAsync(id);

            if (teacher is null)
            {
                _logger.LogWarning("Teacher with id:{Id} Not Found", id);
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

            teacher.BirthDate = new DateTime(teacher.BirthDate.Year, teacher.BirthDate.Month, teacher.BirthDate.Day);

            var result = await _unitOfWork.Teacher.AddAsync(teacher);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction("GetTeacher", new { id = result.Id }, result.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TeacherDto>> UpdateTeacherAsync(int id, UpdateTeacherDto teacherDto)
        {
            var existingTeacher = await _unitOfWork.Teacher.GetByIdAsync(id);

            if (existingTeacher is null)
            {
                _logger.LogWarning("Teacher with id:{Id} Not Found", id);
                return NotFound();
            }

            existingTeacher.FirstName = teacherDto.FirstName;
            existingTeacher.LastName = teacherDto.LastName;
            //existingTeacher.BirthDate = teacherDto.BirthDate;
            existingTeacher.BirthDate = new DateTime(teacherDto.BirthDate.Year, teacherDto.BirthDate.Month, teacherDto.BirthDate.Day);

            await _unitOfWork.Teacher.UpdateAsync(existingTeacher);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTeacherAsync(int id)
        {
            var teacher = await _unitOfWork.Teacher.GetByIdAsync(id);

            if (teacher is null)
            {
                _logger.LogWarning("Teacher with id:{Id} Not Found", id);
                return NotFound();
            }

            //await _teacherRepo.RemoveAsync(id);
            await _unitOfWork.Teacher.RemoveAsync(id);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}
