using Framework.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Persistance
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly DbContext dbContext;
        protected readonly Guid? userId;
        protected readonly List<string> roles;
        protected readonly Dictionary<Type, object> repositories;

        private IDbContextTransaction _transaction;

        public UnitOfWork(DbContext dbContext)
        {
            this.dbContext = dbContext;
            this.userId = null;
            this.roles = new List<string>();
        }

        public UnitOfWork(DbContext dbContext, Guid userId, List<string> roles)
        {
            this.dbContext = dbContext;
            this.userId = userId;
            this.roles = roles;
        }


        public virtual IGenericRepository<T> GetRepository<T>() where T : class
        {
            if (repositories.ContainsKey(typeof(T)))
            {
                return (IGenericRepository<T>)repositories[typeof(T)];
            }

            var repository = new GenericRepository<T>(dbContext);
            repositories.Add(typeof(T), repository);
            return repository;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            await _transaction.RollbackAsync(cancellationToken);
        }

        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await dbContext.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            dbContext.Dispose();
        }
    }
}
