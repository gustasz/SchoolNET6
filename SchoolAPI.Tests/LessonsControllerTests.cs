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
    public class LessonsControllerTests
    {
        private readonly LessonsController _sut;
        private readonly Mock<ILessonRepository> _lessonRepoMock = new Mock<ILessonRepository>();
        private readonly Mock<ICourseRepository> _courseRepoMock = new Mock<ICourseRepository>();
        public LessonsControllerTests()
        {
            _sut = new LessonsController(_lessonRepoMock.Object,_courseRepoMock.Object);
        }

        [Fact]
        public async Task AddLessonsAsync_ReturnsBadRequest_WhenThereIsScheduleOverlapForAnyStudent()
        {
            //Arrange
            var courseId = 50;
            var course = new Course { Id = 50 };
            var courseStudents = new List<Student>
            {
                new Student{Id = 4, FirstName = "Jack", LastName = "Sparrow", BirthDate = DateTime.Parse("2004-02-03"), Grade = 10 }
            };
            var studentLessons = new List<Lesson> // lessons the student already has
            {
                new Lesson{Id = 14, Time = DateTime.Parse("2022-06-09 08:00:00")}
            };

            _courseRepoMock.Setup(x => x.GetCourseAsync(courseId)).
                ReturnsAsync(course);
            _courseRepoMock.Setup(x => x.GetCourseStudentsAsync(courseId))
                .ReturnsAsync(courseStudents);
            _lessonRepoMock.Setup(x => x.GetStudentLessonsAsync(4))
                .ReturnsAsync(studentLessons);
            //Act
            CreateLessonShortDto[] lessonToAdd = new CreateLessonShortDto[]
            {
                new CreateLessonShortDto(DateTime.Parse("2022-06-09 15:14:13"),1) // trying to add a lesson at 2022-06-09 08:00:00
            };
            var response = await _sut.AddLessonsAsync(lessonToAdd,courseId);
            //Assert
            Assert.NotNull(response);
            Assert.IsType<BadRequestObjectResult>(response.Result);
            Assert.StartsWith("Schedule overlap", (response.Result as ObjectResult)?.Value.ToString());
        }

        [Fact]
        public async Task UpdateLessonAsync_ReturnsBadRequest_WhenThereIsScheduleOverlapForAnyStudent()
        {
            //Arrange
            var courseId = 50;
            var lessonId = 6;
            var course = new Course { Id = courseId };
            var lesson = new Lesson { Id = lessonId };
            var courseStudents = new List<Student>
            {
                new Student{Id = 4, FirstName = "Jack", LastName = "Sparrow", BirthDate = DateTime.Parse("2004-02-03"), Grade = 10 }
            };
            var studentLessons = new List<Lesson> // lessons the student already has
            {
                new Lesson{Id = 14, Time = DateTime.Parse("2022-06-09 08:00:00")}
            };

            _courseRepoMock.Setup(x => x.GetCourseAsync(courseId))
                .ReturnsAsync(course);
            _courseRepoMock.Setup(x => x.GetCourseStudentsAsync(courseId))
                .ReturnsAsync(courseStudents);
            _lessonRepoMock.Setup(x => x.GetLessonAsync(lessonId))
                .ReturnsAsync(lesson);
            _lessonRepoMock.Setup(x => x.GetStudentLessonsAsync(4))
                .ReturnsAsync(studentLessons);
            //Act
            UpdateLessonDto lessonToUpdate = new UpdateLessonDto(courseId, DateTime.Parse("2022-06-09 15:14:13"), 1);
            var response = await _sut.UpdateLessonAsync(lessonId,lessonToUpdate);
            //Assert
            Assert.NotNull(response);
            Assert.IsType<BadRequestObjectResult>(response.Result);
            Assert.StartsWith("Schedule overlap", (response.Result as ObjectResult)?.Value.ToString());
        }
    }
}
