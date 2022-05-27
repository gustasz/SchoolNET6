using System.ComponentModel.DataAnnotations;

namespace SchoolAPI
{
    public record StudentDto(int Id, string FirstName, string LastName, DateTime BirthDate, [Range(1,13)] int Grade);
    public record CreateStudentDto(string FirstName, string LastName, DateTime BirthDate, [Range(1, 13)] int Grade);
    public record UpdateStudentDto(string FirstName, string LastName, DateTime BirthDate, [Range(1, 13)] int Grade);
    public record TeacherDto(int Id, string FirstName, string LastName, DateTime BirthDate);
    public record CreateTeacherDto(string FirstName, string LastName, DateTime BirthDate);
    public record UpdateTeacherDto(string FirstName, string LastName, DateTime BirthDate);
    public record CourseDto(int Id, string SubjectName, string teacherName);
    public record CreateCourseDto(int subjectId, int teacherId);
    public record UpdateCourseDto(int subjectId, int teacherId);
    public record SubjectDto(int Id, string Name);
    public record CreateSubjectDto(string Name);
    public record UpdateSubjectDto(string Name);
}
