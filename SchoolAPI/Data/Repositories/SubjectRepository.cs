using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SchoolAPI.Data.Repositories;
using SchoolAPI.Models;

namespace SchoolAPI.Data
{
    public class SubjectRepository : GenericRepository<Subject>, ISubjectRepository
    {
        public SubjectRepository(SchoolContext context) : base(context) { }
        /*private readonly SchoolContext _context;
        private readonly IMemoryCache _memoryCache;
        public SubjectRepository(SchoolContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
        public async Task<IEnumerable<Subject>> GetSubjectsAsync()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.Subjects, out List<Subject> subjectList))
            {
                subjectList = await _context.Subjects.AsNoTracking().ToListAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));

                _memoryCache.Set(CacheKeys.Subjects, subjectList, cacheEntryOptions);
            }
            return subjectList;
        }

        public async Task<Subject> GetSubjectAsync(int subjectId)
        {
            return await _context.Subjects.FirstOrDefaultAsync(s => s.Id == subjectId);
        }

        public async Task<Subject> AddSubjectAsync(Subject subject)
        {
            var result = await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();
            _memoryCache.Remove(CacheKeys.Subjects);
            return result.Entity;
        }

        public async Task<Subject> UpdateSubjectAsync(Subject subject)
        {
            var result = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == subject.Id);

            result.Name = subject.Name;

            await _context.SaveChangesAsync();
            _memoryCache.Remove(CacheKeys.Subjects);
            return result;

        }

        public async Task DeleteSubjectAsync(int subjectId)
        {
            var result = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == subjectId);
            _context.Subjects.Remove(result);
            await _context.SaveChangesAsync();
            _memoryCache.Remove(CacheKeys.Subjects);
        }*/
    }
}
