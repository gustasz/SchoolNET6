using System.ComponentModel.DataAnnotations;

namespace SchoolAPI
{
    public record StudentDto(int Id, string FullName, DateTime BirthDate, string FullGrade);
    public record CreateStudentDto(string FirstName, string LastName, DateTime BirthDate, [Range(1, 12)] int Grade, [Range(0, 4)] int Class);
    public record UpdateStudentDto(string FirstName, string LastName, DateTime BirthDate, [Range(1, 12)] int Grade, [Range(0, 4)] int Class);
    public record TeacherDto(int Id, string FullName, DateTime BirthDate);
    public record CreateTeacherDto(string FirstName, string LastName, DateTime BirthDate);
    public record UpdateTeacherDto(string FirstName, string LastName, DateTime BirthDate);
    public record CourseDto(int Id, int SubjectId, string SubjectName, int ForGrade, int ForClass, int TeacherId, string TeacherName, int StudentCount, int LessonCount);
    public record CreateCourseDto(int SubjectId, [Range(1, 12)] int ForGrade, [Range(0, 4)] int ForClass, int TeacherId);
    public record UpdateCourseDto(int SubjectId, [Range(1, 12)] int ForGrade, [Range(0, 4)] int ForClass, int TeacherId);
    public record SubjectDto(int Id, string Name);
    public record CreateSubjectDto(string Name);
    public record UpdateSubjectDto(string Name);
    public record LessonDto(int Id, int CourseId, DateTime Time);
    //public record CreateLessonDto(int courseId, DateTime day, [Range(1, 8)] int lessonOfTheDay);
    public record UpdateLessonDto(int CourseId, DateTime DayDate, [Range(1, 8)] int LessonOfTheDay);
    public record CreateLessonShortDto(DateTime DayDate, [Range(1,8)] int LessonOfTheDay);
}
