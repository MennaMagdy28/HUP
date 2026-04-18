using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Application
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}