namespace HUP.BuildingBlocks.Application { public interface IUnitOfWork : IDisposable { Task<int> SaveChangesAsync(CancellationToken cancellationToken = default); } }
