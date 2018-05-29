using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace crossblog.Repositories
{
    public interface IGenericRepository<T>
    {
        Task<T> GetAsync(int id);

        Task<T> GetAsyncReadOnly(int id);

        IQueryable<T> Query();

        Task InsertAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(int id);
    }
}