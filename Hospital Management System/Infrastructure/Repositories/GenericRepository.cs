using Hospital_Management_System.Application.Common.Interfaces; 
using Hospital_Management_System.Application.Common.Models; 
using Hospital_Management_System.Domain.Common; 
using Hospital_Management_System.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore; 
using System.Linq.Expressions; 
namespace Hospital_Management_System.Infrastructure.Repositories
{
    public class GenericRepository<T>(ApplicationDbContext context) : IGenericRepository<T> where T : BaseEntity 
    {
        private readonly ApplicationDbContext _context = context; 
        private readonly DbSet<T> _dbSet = context.Set<T>(); 

        private static IQueryable<T> ApplyIncludes(IQueryable<T> query, Expression<Func<T, object>>[] includes) 
        {
            foreach (var include in includes) 
                query = query.Include(include); 
            return query; 
        }

        public async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes) 
        {
            var query = ApplyIncludes(_dbSet, includes); 
            return await query.FirstOrDefaultAsync(e => e.Id == id); 
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, params Expression<Func<T, object>>[] includes)
        {
            var query = ApplyIncludes(_dbSet.AsNoTracking(), includes); 

            if (filter != null) 
                query = query.Where(filter); 

            return await query.ToListAsync(); 
        }

        public async Task<PagedResult<T>> GetPagedAsync( 
            PaginationParams pagination,
            Expression<Func<T, bool>>? filter = null, 
            Expression<Func<T, object>>? orderBy = null, 
            bool descending = false,
            params Expression<Func<T, object>>[] includes) 
        {
            var query = ApplyIncludes(_dbSet.AsNoTracking(), includes); 

            if (filter != null) 
                query = query.Where(filter); 

            var totalCount = await query.CountAsync(); 

            IOrderedQueryable<T> ordered; 
            if (orderBy == null) 
                ordered = query.OrderBy(e => e.Id);
            else if (descending) 
                ordered = query.OrderByDescending(orderBy);
            else
                ordered = query.OrderBy(orderBy); 

            var pageNumber = Math.Max(1, pagination.PageNumber); 
            var items = await ordered 
                .Skip((pageNumber - 1) * pagination.PageSize) 
                .Take(pagination.PageSize) 
                .ToListAsync(); 

            return new PagedResult<T>(items, totalCount, pageNumber, pagination.PageSize); 
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter) 
        {
            return await _dbSet.AnyAsync(filter); 
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null) 
        {
            return filter == null 
                ? await _dbSet.CountAsync() 
                : await _dbSet.CountAsync(filter); 
        }

        public async Task AddAsync(T entity) 
        {
            await _dbSet.AddAsync(entity); 
        }

        public void Update(T entity) 
        {
            _dbSet.Update(entity); 
        }

        public void Delete(T entity) 
        {
            _dbSet.Remove(entity); 
        }
    }
}
