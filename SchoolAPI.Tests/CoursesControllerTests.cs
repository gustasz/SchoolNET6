using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SchoolAPI.Controllers;
using SchoolAPI.Data;
using SchoolAPI.Data.Interfaces;
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
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ICourseRepository> _courseRepoMock = new();
        private readonly Mock<ISubjectRepository> _subjectRepoMock = new();
        private readonly Mock<ITeacherRepository> _teacherRepoMock = new();
        private readonly Mock<IStudentRepository> _studentRepoMock = new();
        private readonly Mock<ILessonRepository> _lessonRepoMock = new();
        private readonly Mock<ILogger<CoursesController>> _loggerRepoMock = new();
        public CoursesControllerTests()
        {
            _unitOfWorkMock.Setup(m => m.Course).Returns(_courseRepoMock.Object);
            _unitOfWorkMock.Setup(m => m.Subject).Returns(_subjectRepoMock.Object);
            _unitOfWorkMock.Setup(m => m.Teacher).Returns(_teacherRepoMock.Object);
            _unitOfWorkMock.Setup(m => m.Student).Returns(_studentRepoMock.Object);
            _unitOfWorkMock.Setup(m => m.Lesson).Returns(_lessonRepoMock.Object);
            _sut = new CoursesController(_unitOfWorkMock.Object, _loggerRepoMock.Object);
        }

        [Fact]
        public async Task AddClassToCourse_ReturnsBadRequest_WhenAnyStudentHasScheduleOverlap()
        {
            // Arrange
            List<Lesson> firstLessons = new() { new Lesson { Time = DateTime.Parse("2022-06-12 15:00")} };
            List<Lesson> courseLessons = new() { new Lesson { Time = DateTime.Parse("2022-06-12 15:00") } };
            List<Student> students = new() { new Student { Id = 4, Grade = 2, Class = 1} };

            var course = new Course { Id = 50, ForGrade = 2, ForClass = 1 };
            _lessonRepoMock.Setup(x => x.GetStudentLessonsAsync(4))
                .ReturnsAsync(firstLessons);
            _lessonRepoMock.Setup(x => x.GetCourseLessonsAsync(50))
                .ReturnsAsync(courseLessons);
            _courseRepoMock.Setup(x => x.GetByIdAsync(50))
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
