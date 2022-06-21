using Microsoft.EntityFrameworkCore;
using SchoolAPI.Data.Interfaces;

namespace SchoolAPI.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected SchoolContext _context;
        public GenericRepository(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T> AddAsync(T entity)
        {
            var result = await _context.Set<T>().AddAsync(entity);
            return result.Entity;
        }

        public async Task UpdateAsync(T entity)
        {
             _context.Entry<T>(entity).State = EntityState.Modified;
            //_context.Set<T>().Update(entity);
        }
        public async Task RemoveAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            _context.Set<T>().Remove(entity);
        }
    }
}
