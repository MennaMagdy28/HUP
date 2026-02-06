using HUP.Core.Entities.Academics;
using HUP.Data;
using HUP.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HUP.Repositories.Implementations
{
    public class ScheduleRepository : IScheduleRepository
    {

        private readonly HupDbContext _context;
        public ScheduleRepository(HupDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Schedule entity)
        {
            await _context.Schedules.AddAsync(entity);
        }
        public async Task<IEnumerable<Schedule>> GetAllAsync()
        {
            return await _context.Schedules.AsNoTracking().ToListAsync();
        }

        public async Task<Schedule> GetByIdReadOnly(Guid id)
        {
            var s = await _context.Schedules
                .Where(s => s.Id == id)
                .Include(s => s.CourseOffering)
                .Include(s => s.CourseOffering.Course)
                .AsNoTracking().FirstOrDefaultAsync();
            return s;
        }
        public async Task<Schedule> GetByIdTracking(Guid id)
        {
            var s = await _context.Schedules.FirstOrDefaultAsync(s => s.Id == id);
            return s;
        }


        public async Task RemoveAsync(Guid id)
        {
            var entity = await _context.Schedules.FindAsync(id);
            
            if (entity != null)
                _context.Schedules.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }  
    }
}
