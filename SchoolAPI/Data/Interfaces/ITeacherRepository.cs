using SchoolAPI.Data.Interfaces;
using SchoolAPI.Models;

namespace SchoolAPI.Data
{
    public interface ITeacherRepository : IGenericRepository<Teacher>
    {
        // can implement custom repo methods
    }
}
