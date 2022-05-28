﻿using Microsoft.EntityFrameworkCore;
using SchoolAPI.Models;

namespace SchoolAPI.Data
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly SchoolContext _context;
        public TeacherRepository(SchoolContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Teacher>> GetTeachersAsync()
        {
            return await _context.Teachers.ToListAsync();
        }

        public async Task<Teacher> GetTeacherAsync(int teacherId)
        {
            return await _context.Teachers.FirstOrDefaultAsync(t => t.Id == teacherId);
        }

        public async Task<Teacher> AddTeacherAsync(Teacher teacher)
        {
            var result = await _context.Teachers.AddAsync(teacher);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Teacher> UpdateTeacherAsync(Teacher teacher)
        {
            var result = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == teacher.Id);

            if (result is not null)
            {
                result.FirstName = teacher.FirstName;
                result.LastName = teacher.LastName;
                result.BirthDate = teacher.BirthDate;

                await _context.SaveChangesAsync();

                return result;
            }

            return null;
        }

        public async Task DeleteTeacherAsync(int teacherId)
        {
            var result = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == teacherId);
            if (result is not null)
            {
                _context.Teachers.Remove(result);
                await _context.SaveChangesAsync();
            }
        }
    }
}