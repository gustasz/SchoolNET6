using SchoolAPI.Models;

namespace SchoolAPI
{
    public static class Extensions
    {
        public static readonly string[] classesAsString = { "", "A", "B", "C", "D" }; // classes in database are stored as int 0-4, depends on the customer on how to display it
        public static StudentDto AsDto(this Student student)
        {
            return new StudentDto(student.Id, $"{student.FirstName} {student.LastName}", student.BirthDate.Date, $"{student.Grade}{classesAsString[student.Class]}");
        }

        public static TeacherDto AsDto(this Teacher teacher)
        {
            return new TeacherDto(teacher.Id, $"{teacher.FirstName} {teacher.LastName}", teacher.BirthDate);
        }

        public static CourseDto AsDto(this Course course)
        {
            return new CourseDto(course.Id, course.Subject.Id, course.Subject.Name, course.ForGrade,course.ForClass, course.Teacher.Id, course.Teacher.FirstName + " " + course.Teacher.LastName, course.Students.Count, course.Lessons.Count);
        }

        public static SubjectDto AsDto(this Subject subject)
        {
            return new SubjectDto(subject.Id, subject.Name);
        }

        public static LessonDto AsDto(this Lesson lesson)
        {
            return new LessonDto(lesson.Id, lesson.Course.Id, lesson.Time);
        }

        public static List<LessonDto> AsDtos(this List<Lesson> lessons)
        {
            var lessonsDto = new List<LessonDto>();
            foreach(var lesson in lessons)
            {
                lessonsDto.Add(lesson.AsDto());
            }
            return lessonsDto;
        }
    }
}
