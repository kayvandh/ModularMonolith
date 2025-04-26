using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Persistance.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default);

        Task<ListResult<T>> GetAllAsync(
            PageRequest? pageRequest = null,
            CancellationToken cancellationToken = default,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            params Expression<Func<T, object>>[] includes);
        Task<ListResult<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            PageRequest? pageRequest = null,
            CancellationToken cancellationToken = default,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            params Expression<Func<T, object>>[] includes);
        Task<ListResult<T>> FindAsync(
            ISpecification<T> specification,
            PageRequest? pageRequest = null,
            CancellationToken cancellationToken = default);

        Task<T?> GetOneAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default,
            params Expression<Func<T, object>>[] includes);

        Task<IQueryable<T>> GetQueryableAsync(
            CancellationToken cancellationToken = default,
            params Expression<Func<T, object>>[] includes);

        Task<int> CountAsync(
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken cancellationToken = default);

        Task<decimal> SumAsync(
            Expression<Func<T, decimal>> selector,
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken cancellationToken = default);
        Task<decimal> AvgAsync(
            Expression<Func<T, decimal>> selector, 
            Expression<Func<T, bool>>? predicate = null, 
            CancellationToken cancellationToken = default);

        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        void Update(T entity);
        void Delete(T entity);
    }

}
