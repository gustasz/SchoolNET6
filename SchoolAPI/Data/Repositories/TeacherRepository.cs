using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SchoolAPI.Data.Repositories;
using SchoolAPI.Models;

namespace SchoolAPI.Data
{
    public class TeacherRepository : GenericRepository<Teacher>, ITeacherRepository
    {
        public TeacherRepository(SchoolContext _context) : base(_context) { }
       /* private readonly SchoolContext _context;
        private readonly IMemoryCache _memoryCache;
        public TeacherRepository(SchoolContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
        public async Task<IEnumerable<Teacher>> GetTeachersWithInfoAsync()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.Teachers, out List<Teacher> teacherList))
            {
                teacherList = await _context.Teachers.AsNoTracking().ToListAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));

                _memoryCache.Set(CacheKeys.Teachers, teacherList, cacheEntryOptions);
            }
            return teacherList;
        }

        public async Task<Teacher> GetTeacherAsync(int teacherId)
        {
            return await _context.Teachers.FirstOrDefaultAsync(t => t.Id == teacherId);
        }

        public async Task<Teacher> AddTeacherAsync(Teacher teacher)
        {
            teacher.BirthDate = new DateTime(teacher.BirthDate.Year,teacher.BirthDate.Month,teacher.BirthDate.Day);
            var result = await _context.Teachers.AddAsync(teacher);
            await _context.SaveChangesAsync();
            _memoryCache.Remove(CacheKeys.Teachers);
            return result.Entity;
        }

        public async Task<Teacher> UpdateTeacherAsync(Teacher teacher)
        {
            var result = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == teacher.Id);

            result.FirstName = teacher.FirstName;
            result.LastName = teacher.LastName;
            result.BirthDate = teacher.BirthDate;

            await _context.SaveChangesAsync();
            _memoryCache.Remove(CacheKeys.Teachers);

            return result;
        }

        public async Task DeleteTeacherAsync(int teacherId)
        {
            var result = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == teacherId);
            _context.Teachers.Remove(result);
            await _context.SaveChangesAsync();
            _memoryCache.Remove(CacheKeys.Teachers);
        }*/
    }
}
