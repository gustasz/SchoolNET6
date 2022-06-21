using Microsoft.AspNetCore.Mvc;
using SchoolAPI.Data;
using SchoolAPI.Data.Interfaces;
using SchoolAPI.Models;

namespace SchoolAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SubjectsController> _logger;
        public SubjectsController(IUnitOfWork unitOfWork, ILogger<SubjectsController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<SubjectDto>> GetSubjectsAsync()
        {
            var subjects = (await _unitOfWork.Subject.GetAllAsync())
                            .Select(subject => subject.AsDto());
            return subjects;

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SubjectDto>> GetSubjectAsync(int id)
        {
            var subject = await _unitOfWork.Subject.GetByIdAsync(id);

            if (subject is null)
            {
                _logger.LogWarning("Subject with id:{Id} Not Found",id);
                return NotFound();
            }

            return subject.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<SubjectDto>> CreateSubjectAsync(CreateSubjectDto subjectDto)
        {
            Subject subject = new()
            {
                Name = subjectDto.Name
            };

            var item = await _unitOfWork.Subject.AddAsync(subject);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction("GetSubject", new { id = item.Id }, item.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SubjectDto>> UpdateSubjectAsync(int id, UpdateSubjectDto subjectDto)
        {
            var existingSubject = await _unitOfWork.Subject.GetByIdAsync(id);

            if (existingSubject is null)
            {
                _logger.LogWarning("Subject with id:{Id} Not Found", id);
                return NotFound();
            }

            existingSubject.Name = subjectDto.Name;

            await _unitOfWork.Subject.UpdateAsync(existingSubject);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSubjectAsync(int id)
        {
            var subject = await _unitOfWork.Subject.GetByIdAsync(id);

            if (subject is null)
            {
                _logger.LogWarning("Subject with id:{Id} Not Found", id);
                return NotFound();
            }

            await _unitOfWork.Subject.RemoveAsync(id);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}
