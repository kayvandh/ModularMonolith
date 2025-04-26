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
        private readonly DbSet<T> dbSet;
        private readonly DbContext dbContext;

        //public delegate Task<Expression<Func<T, bool>>> SecurityPredicateDelegate(CancellationToken cancellationToken);
        //private readonly SecurityPredicateDelegate _applySecurityPredicate;

        private readonly Func<CancellationToken, Task<Expression<Func<T, bool>>>>? securityFilterFunc;


        public GenericRepository(DbContext dbContext)
        {
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<T>();            
        }

        public GenericRepository(DbContext dbContext, Func<CancellationToken, Task<Expression<Func<T, bool>>>>? securityFilterFunc = null)
        {
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<T>();
            this.securityFilterFunc = securityFilterFunc;
        }

        public async Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            return await dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<ListResult<T>> GetAllAsync(
              PageRequest? pageRequest = null,
              CancellationToken cancellationToken = default,
              Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
              params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = dbSet.AsQueryable();

            query = await ApplySecurityFilterAsync(query, cancellationToken);

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
            var query = dbSet.AsQueryable();

            query = await ApplySecurityFilterAsync(query, cancellationToken);
            query = query.Where(predicate);

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
            IQueryable<T> query = dbSet.AsQueryable();

            query = await ApplySecurityFilterAsync(query, cancellationToken);

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
            IQueryable<T> query = dbSet.AsQueryable();

            query = await ApplySecurityFilterAsync(query, cancellationToken);
            query = query.Where(predicate);

            query = ApplyIncludes(query, includes);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IQueryable<T>> GetQueryableAsync(
            CancellationToken cancellationToken = default,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = dbSet.AsQueryable();
            query = await ApplySecurityFilterAsync(query, cancellationToken);
            query = ApplyIncludes(query, includes);
            return query;
        }

        public async Task<int> CountAsync(
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = dbSet.AsQueryable();

            query = await ApplySecurityFilterAsync(query, cancellationToken);

            if (predicate != null)
                query = query.Where(predicate);

            return await query.CountAsync(cancellationToken);
        }

        public async Task<decimal> SumAsync(
            Expression<Func<T, decimal>> selector, 
            Expression<Func<T, bool>>? predicate = null, 
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = dbSet.AsQueryable();

            query = await ApplySecurityFilterAsync(query, cancellationToken);

            if (predicate != null)
                query = query.Where(predicate);

            return await query.SumAsync(selector, cancellationToken);
        }

        public async Task<decimal> AvgAsync(
            Expression<Func<T, decimal>> selector, 
            Expression<Func<T, bool>>? predicate = null, 
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = dbSet.AsQueryable();

            query = await ApplySecurityFilterAsync(query, cancellationToken);
            if (predicate != null)
                query = query.Where(predicate);

            return await query.AverageAsync(selector, cancellationToken);

        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await dbContext.Set<T>().AddAsync(entity, cancellationToken);
        }

        public void Update(T entity)
        {
            dbSet.Update(entity);
            
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);            
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

        private async Task<IQueryable<T>> ApplySecurityFilterAsync(IQueryable<T> query, CancellationToken cancellationToken)
        {
            if (securityFilterFunc != null)
            {
                var predicate = await securityFilterFunc(cancellationToken);
                if (predicate != null)
                    query = query.Where(predicate);
            }
            return query;
        }

    }
}
