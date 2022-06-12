using Microsoft.AspNetCore.Mvc;
using Moq;
using SchoolAPI.Controllers;
using SchoolAPI.Data;
using SchoolAPI.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace SchoolAPI.Tests
{
    public class CoursesControllerTests
    {
        private readonly CoursesController _sut;
        private readonly Mock<ICourseRepository> _courseRepoMock = new Mock<ICourseRepository>();
        private readonly Mock<ISubjectRepository> _subjectRepoMock = new Mock<ISubjectRepository>();
        private readonly Mock<ITeacherRepository> _teacherRepoMock = new Mock<ITeacherRepository>();
        private readonly Mock<IStudentRepository> _studentRepoMock = new Mock<IStudentRepository>();
        private readonly Mock<ILessonRepository> _lessonRepoMock = new Mock<ILessonRepository>();
        public CoursesControllerTests()
        {
            _sut = new CoursesController(_courseRepoMock.Object, _subjectRepoMock.Object, _teacherRepoMock.Object, _studentRepoMock.Object, _lessonRepoMock.Object);
        }

        [Fact]
        public async Task AddClassToCourse_ReturnsBadRequest_WhenAnyStudentHasScheduleOverlap()
        {
            // Arrange
            List<Lesson> firstLessons = new List<Lesson> { new Lesson { Time = DateTime.Parse("2022-06-12 15:00")} };
            List<Lesson> courseLessons = new List<Lesson> { new Lesson { Time = DateTime.Parse("2022-06-12 15:00") } };
            List<Student> students = new List<Student> { new Student { Id = 4, Grade = 2, Class = 1} };

            var course = new Course { Id = 50, ForGrade = 2, ForClass = 1 };
            _lessonRepoMock.Setup(x => x.GetStudentLessonsAsync(4))
                .ReturnsAsync(firstLessons);
            _lessonRepoMock.Setup(x => x.GetCourseLessonsAsync(50))
                .ReturnsAsync(courseLessons);
            _courseRepoMock.Setup(x => x.GetCourseAsync(50))
                .ReturnsAsync(course);
            _studentRepoMock.Setup(x => x.GetStudentsFromClassAsync(2,1))
                .ReturnsAsync(students);
            // Act
            var studentId = 5;
            var response = await _sut.AddClassToCourse(50, 2, 1);
            // Assert
            Assert.NotNull(response);
            Assert.IsType<BadRequestObjectResult>(response.Result);
            Assert.StartsWith("Schedule overlap", (response.Result as ObjectResult)?.Value.ToString());
        }
    }
}
