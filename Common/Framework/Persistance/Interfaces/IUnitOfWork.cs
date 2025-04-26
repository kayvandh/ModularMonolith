using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Persistance.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> GetRepository<T>() where T : class;
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);  
        Task CommitAsync(CancellationToken cancellationToken = default);  
        Task RollbackAsync(CancellationToken cancellationToken = default);         
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
