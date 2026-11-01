using System.Linq.Expressions; 
using Hospital_Management_System.Application.Common.Models;
using Hospital_Management_System.Domain.Common;

namespace Hospital_Management_System.Application.Common.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes); 

        Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, params Expression<Func<T, object>>[] includes);

        Task<PagedResult<T>> GetPagedAsync( 
            PaginationParams pagination, 
            Expression<Func<T, bool>>? filter = null, 
            Expression<Func<T, object>>? orderBy = null, 
            bool descending = false, 
            params Expression<Func<T, object>>[] includes); 

        Task<bool> AnyAsync(Expression<Func<T, bool>> filter); 

        Task<int> CountAsync(Expression<Func<T, bool>>? filter = null); 

        Task AddAsync(T entity); 

        void Update(T entity); 

        void Delete(T entity);
    }
}
