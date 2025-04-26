using Framework.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Persistance
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DbSet<T> _dbSet;
        private readonly DbContext _dbContext;
        public GenericRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public async Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<ListResult<T>> GetAllAsync(
              PageRequest? pageRequest = null,
              CancellationToken cancellationToken = default,
              Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
              params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.AsQueryable();
            query = ApplyIncludes(query, includes);

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            int totalCount = await query.CountAsync(cancellationToken);

            if (pageRequest != null)
            {
                query = query.Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
                             .Take(pageRequest.PageSize);
            }

            var items = await query.ToListAsync(cancellationToken);

            return new ListResult<T>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public async Task<ListResult<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            PageRequest? pageRequest = null,
            CancellationToken cancellationToken = default,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.Where(predicate);
            query = ApplyIncludes(query, includes);

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            int totalCount = await query.CountAsync(cancellationToken);

            if (pageRequest != null)
            {
                query = query.Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
                             .Take(pageRequest.PageSize);
            }

            var items = await query.ToListAsync(cancellationToken);

            return new ListResult<T>
            {
                Items = items,
                TotalCount = totalCount
            };
        }


        public async Task<ListResult<T>> FindAsync(
            ISpecification<T> specification,
            PageRequest? pageRequest = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            query = ApplyIncludes(query, specification.Includes.ToArray());

            if (specification.OrderBy != null)
            {
                query = specification.OrderBy(query);
            }

            int totalCount = await query.CountAsync(cancellationToken);

            if (pageRequest != null)
            {
                query = query.Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
                             .Take(pageRequest.PageSize);
            }

            var items = await query.ToListAsync(cancellationToken);

            return new ListResult<T>
            {
                Items = items,
                TotalCount = totalCount
            };
        }


        public async Task<T?> GetOneAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.Where(predicate);

            query = ApplyIncludes(query, includes);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public  IQueryable<T> GetQueryable(
            CancellationToken cancellationToken = default,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            query = ApplyIncludes(query, includes);

            return query;
        }

        public async Task<int> CountAsync(
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            if (predicate != null)
                return await _dbSet.CountAsync(predicate, cancellationToken);

            return await _dbSet.CountAsync(cancellationToken);
        }

        public async Task<decimal> SumAsync(
            Expression<Func<T, decimal>> selector,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet.SumAsync(selector, cancellationToken);

        }

        public async Task<decimal> AverageAsync(
            Expression<Func<T, decimal>> selector,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet.AverageAsync(selector, cancellationToken);

        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
            
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);            
        }

        //

        private IQueryable<T> ApplyIncludes(IQueryable<T> query, params Expression<Func<T, object>>[] includes)
        {
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return query;
        }

    }
}
