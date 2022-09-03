using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolAPI.Data;
using SchoolAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SchoolAPI.Tests
{
    public class SubjectRepositoryTests
    {
        [Fact]
        public async Task SubjectRepository_ListsSubjectsFromDatabase()
        {
            DbContextOptionsBuilder<SchoolContext> optionsBuilder = new();
            optionsBuilder.UseInMemoryDatabase(MethodBase.GetCurrentMethod().Name);

            using (SchoolContext ctx = new(optionsBuilder.Options))
            {
                ctx.Add(new Subject { Id = 1, Name = "Foo"});
                ctx.SaveChanges();
            }

                IEnumerable<Subject> result;
            using (SchoolContext ctx = new(optionsBuilder.Options))
            {
                result = await new SubjectRepository(ctx).GetAllAsync();
            }

            Assert.NotNull(result);
            var subjects = Assert.IsType<List<Subject>>(result);
            var subject = Assert.Single(subjects);
            Assert.Equal(1, subject.Id);
            Assert.Equal("Foo", subject.Name);
        }

        [Fact]
        public async Task SubjectRepository_GetsSubjectFromDatabase()
        {
            DbContextOptionsBuilder<SchoolContext> optionsBuilder = new();
            optionsBuilder.UseInMemoryDatabase(MethodBase.GetCurrentMethod().Name);

            using (SchoolContext ctx = new(optionsBuilder.Options))
            {
                ctx.Add(new Subject { Id = 1, Name = "Foo" });
                ctx.SaveChanges();
            }

            Subject result;
            using (SchoolContext ctx = new(optionsBuilder.Options))
            {
                result = await new SubjectRepository(ctx).GetByIdAsync(1);
            }

            var subject = Assert.IsType<Subject>(result);
            Assert.NotNull(result);
            Assert.Equal(1, subject.Id);
            Assert.Equal("Foo", subject.Name);
        }
    }
}
