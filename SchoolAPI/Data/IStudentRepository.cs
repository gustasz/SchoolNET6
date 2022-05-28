﻿using SchoolAPI.Models;

namespace SchoolAPI.Data
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetStudentsAsync();
        Task<Student> GetStudentAsync(int studentId);
        Task<Student> AddStudentAsync(Student student);
        Task<Student> UpdateStudentAsync(Student student);
        Task DeleteStudentAsync(int studentId);
        Task<IEnumerable<Course>> GetStudentCoursesAsync(int studentId);
        Task<Student> AddCourseToStudent(int studentId,int courseId);
        Task DeleteCourseFromStudent(int studentId, int courseId);
    }
}