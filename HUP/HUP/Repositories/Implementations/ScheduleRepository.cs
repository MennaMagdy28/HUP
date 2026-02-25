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
                    .ThenInclude(co => co.Course)
                .Include(s => s.Instructor)
                .AsNoTracking().FirstOrDefaultAsync();
            return s;
        }

        public async Task<IEnumerable<Schedule>> GetByStudentEnrollmentsAsync(Guid studentId)
        {
            return await _context.Enrollments
                .Where(e => e.StudentId == studentId && !e.IsDeleted)
                .Where(e => e.CourseOffering.Semester.IsActive) // Filter by active semester
                .SelectMany(e => e.CourseOffering.Schedules)
                .Include(s => s.CourseOffering)
                    .ThenInclude(co => co.Course)
                .Include(s => s.Instructor)
                    .ThenInclude(i => i.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Schedule>> GetAvailableSlotsAsync()
        {
            return await _context.Schedules
                .Where(s => s.AvailableSeats > 0 && !s.IsDeleted)
                .Where(s => s.CourseOffering.Semester.IsActive) // Filter by active semester
                .Include(s => s.CourseOffering)
                    .ThenInclude(co => co.Course)
                .Include(s => s.Instructor)
                    .ThenInclude(i => i.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> TryBookSeatAsync(Guid scheduleId)
        {
            // Raw SQL update for atomicity and concurrency control
            var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                "UPDATE Schedules SET AvailableSeats = AvailableSeats - 1, UpdatedAt = GETUTCDATE() WHERE Id = {0} AND AvailableSeats > 0",
                scheduleId);

            return rowsAffected > 0;
        }
    }
}