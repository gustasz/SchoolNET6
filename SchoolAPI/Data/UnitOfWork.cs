using SchoolAPI.Data.Interfaces;
using SchoolAPI.Data.Repositories;
using SchoolAPI.Models;

namespace SchoolAPI.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SchoolContext _context;
        public ICourseRepository Course { get; }
        public ISubjectRepository Subject { get; }
        public ITeacherRepository Teacher { get; }
        public IStudentRepository Student { get; }
        public ILessonRepository Lesson { get; }

        public UnitOfWork(SchoolContext context,
                          ICourseRepository courseRepository,
                          ISubjectRepository subjectRepository,
                          ITeacherRepository teacherRepository,
                          IStudentRepository studentRepository,
                          ILessonRepository lessonRepository)
        {
            _context = context;
            Course = courseRepository;
            Subject = subjectRepository;
            Teacher = teacherRepository;
            Student = studentRepository;
            Lesson = lessonRepository;
        }
        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
    }
}
