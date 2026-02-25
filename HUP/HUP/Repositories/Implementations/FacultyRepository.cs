using HUP.Core.Entities.Academics;
using HUP.Core.Interfaces;
using HUP.Data;
using HUP.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HUP.Repositories.Implementations
{
    public class FacultyRepository : GenericRepository<Faculty>, IFacultyRepository
    {
        private readonly ICacheService _cacheService;
        private const string CacheKey = "faculties:list";

        public FacultyRepository(HupDbContext context, ICacheService cacheService) : base(context)
        {
            _cacheService = cacheService;
        }

        public override async Task<IEnumerable<Faculty>> GetAllAsync()
        {
            var cached = await _cacheService.GetAsync<IEnumerable<Faculty>>(CacheKey);
            if (cached != null)
            {
                return cached;
            }

            var faculties = await base.GetAllAsync();
            await _cacheService.SetAsync(CacheKey, faculties, 60); // 60 minutes
            return faculties;
        }

        public override async Task AddAsync(Faculty entity)
        {
            await base.AddAsync(entity);
            await _cacheService.RemoveAsync(CacheKey);
        }

        public override async Task RemoveAsync(Guid id)
        {
            await base.RemoveAsync(id);
            await _cacheService.RemoveAsync(CacheKey);
        }
    }
}
