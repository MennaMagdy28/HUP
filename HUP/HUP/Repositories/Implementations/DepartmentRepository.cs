using HUP.Core.Entities.Academics;
using HUP.Core.Interfaces;
using HUP.Data;
using HUP.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HUP.Repositories.Implementations
{
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {
        private readonly ICacheService _cacheService;
        private const string CacheKey = "departments:list";

        public DepartmentRepository(HupDbContext context, ICacheService cacheService) : base(context)
        {
            _cacheService = cacheService;
        }

        public override async Task<IEnumerable<Department>> GetAllAsync()
        {
            var cached = await _cacheService.GetAsync<IEnumerable<Department>>(CacheKey);
            if (cached != null)
            {
                return cached;
            }

            var departments = await base.GetAllAsync();
            await _cacheService.SetAsync(CacheKey, departments, 60); // 60 minutes
            return departments;
        }

        public override async Task AddAsync(Department entity)
        {
            await base.AddAsync(entity);
            await _cacheService.RemoveAsync(CacheKey);
        }

        public override async Task RemoveAsync(Guid id)
        {
            await base.RemoveAsync(id);
            await _cacheService.RemoveAsync(CacheKey);
        }

        public async Task<IEnumerable<Department>> GetByFacultyIdAsync(Guid facultyId)
        {
            // We could cache this too, but for now let's stick to the main list as per request
            // Or we could derive it from the cached full list if it's small enough.
            // But let's keep it simple and just query DB or implement specific cache if needed.
            // The request says "When these lists are requested", implying the main lists.
            return await _context.Departments.Where(d => d.FacultyId == facultyId).ToListAsync();
        }
    }
}
