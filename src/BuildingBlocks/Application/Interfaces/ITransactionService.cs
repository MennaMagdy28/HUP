using System;
using System.Threading.Tasks;

namespace BuildingBlocks.Application.Interfaces
{
    public interface ITransactionService
    {
        Task ExecuteInTransactionAsync(Func<Task> action);
    }
}