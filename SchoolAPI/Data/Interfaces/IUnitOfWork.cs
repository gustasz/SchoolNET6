namespace SchoolAPI.Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICourseRepository Course { get; }
        ISubjectRepository Subject { get; }
        ITeacherRepository Teacher { get; }
        IStudentRepository Student { get; }
        ILessonRepository Lesson { get; }
        Task CompleteAsync();
    }
}
