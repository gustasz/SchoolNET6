using Microsoft.AspNetCore.Mvc;
using SchoolAPI.Data;
using SchoolAPI.Models;

namespace SchoolAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectRepository _repository;
        public SubjectsController(ISubjectRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IEnumerable<SubjectDto>> GetSubjectsAsync()
        {
            var subjects = (await _repository.GetSubjectsAsync())
                            .Select(subject => subject.AsDto());
            return subjects;

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SubjectDto>> GetSubjectAsync(int id)
        {
            var subject = await _repository.GetSubjectAsync(id);

            if (subject is null)
            {
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

            var item = await _repository.AddSubjectAsync(subject);

            return CreatedAtAction("GetSubject", new { id = item.Id }, item.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SubjectDto>> UpdateSubjectAsync(int id, UpdateSubjectDto subjectDto)
        {
            var existingSubject = await _repository.GetSubjectAsync(id);

            if (existingSubject is null)
            {
                return NotFound();
            }

            existingSubject.Name = subjectDto.Name;

            await _repository.UpdateSubjectAsync(existingSubject);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSubjectAsync(int id)
        {
            var existingSubject = await _repository.GetSubjectAsync(id);

            if (existingSubject is null)
            {
                return NotFound();
            }

            await _repository.DeleteSubjectAsync(id);

            return NoContent();
        }
    }
}
