using System.Linq.Expressions;

namespace SchoolAPI.Data.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        //Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task RemoveAsync(int id);

       // Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate);

       // Task<int> CountAll();
       // Task<int> CountWhere(Expression<Func<T, bool>> predicate);
    }
}
