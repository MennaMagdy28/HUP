using System.Threading.Tasks;

namespace BuildingBlocks.Application.Caching
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, int expirationInMinutes);
        Task RemoveAsync(string key);
    }
}