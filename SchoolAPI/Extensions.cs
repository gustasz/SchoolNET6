﻿using SchoolAPI.Models;

namespace SchoolAPI
{
    public static class Extensions
    {
        public static StudentDto AsDto(this Student student)
        {
            return new StudentDto(student.Id, student.FirstName, student.LastName, student.BirthDate, student.Grade);
        }

        public static TeacherDto AsDto(this Teacher teacher)
        {
            return new TeacherDto(teacher.Id, teacher.FirstName, teacher.LastName, teacher.BirthDate);
        }

        public static CourseDto AsDto(this Course course)
        {
            return new CourseDto(course.Id, course.Subject.Name, course.Teacher.FirstName + " " + course.Teacher.LastName);
        }

        public static SubjectDto AsDto(this Subject subject)
        {
            return new SubjectDto(subject.Id, subject.Name);
        }

        public static LessonDto AsDto(this Lesson lesson)
        {
            return new LessonDto(lesson.Id, lesson.Course.Id, lesson.Time);
        }
    }
}
