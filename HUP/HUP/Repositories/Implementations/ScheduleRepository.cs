using HUP.Core.Entities.Academics;
using HUP.Data;
using HUP.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HUP.Repositories.Implementations
{
    public class ScheduleRepository : GenericRepository<Schedule>, IScheduleRepository
    {
        public ScheduleRepository(HupDbContext context) : base(context)
        {
        }

        public override async Task<Schedule> GetByIdReadOnly(Guid id)
        {
            var s = await _context.Schedules
                .Where(s => s.Id == id)
                .Include(s => s.CourseOffering)
                .Include(s => s.CourseOffering.Course)
                .AsNoTracking().FirstOrDefaultAsync();
            return s;
        }
    }
}