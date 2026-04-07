using HUP.Core.Entities.Academics;
using HUP.Core.Models;
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

        public async Task<Schedule> GetByIdWithDetailsAsync(Guid id)
        {
            var s = await _context.Schedules
                .Where(s => s.Id == id)
                .Include(s => s.CourseOffering)
                    .ThenInclude(co => co.Course)
                .Include(s => s.Staff)
                .AsNoTracking().FirstOrDefaultAsync();
            return s;
        }

        public async Task<IEnumerable<ScheduleSlot>> GetByStudentEnrollmentsAsync(Guid studentId)
        {
            var schedules = await _context.Enrollments
                .Where(e => e.StudentId == studentId && !e.IsDeleted)
                .Select(e => new ScheduleSlot
                {
                    SlotId = e.ScheduleId,
                    CourseOfferingId = e.CourseOfferingId ,
                    StaffName = e.Schedule.Staff.User.FullName,
                    CourseName = e.CourseOffering.Course.CourseName,
                    CourseCode = e.CourseOffering.Course.CourseCode,
                    Hall = e.Schedule.Hall,
                    StartTime = e.Schedule.StartTime,
                    EndTime = e.Schedule.EndTime,
                    Group = e.Schedule.Group,
                    DayOfWeek = e.Schedule.DayOfWeek
                })
                .AsNoTracking()
                .ToListAsync();

            return schedules;
        }

        public async Task<IEnumerable<ScheduleSlot>> GetAvailableSlotsAsync()
        {
            return await _context.Schedules
                .Where(s => s.AvailableSeats > 0 && !s.IsDeleted)
                .Where(s => s.CourseOffering.Semester.IsActive) // Filter by active semester
                .Select(s => new ScheduleSlot
                {
                    SlotId = s.Id,
                    CourseOfferingId = s.CourseOfferingId,
                    StaffName = s.Staff.User.FullName,
                    CourseName = s.CourseOffering.Course.CourseName,
                    CourseCode = s.CourseOffering.Course.CourseCode,
                    Hall = s.Hall,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Group = s.Group,
                    DayOfWeek = s.DayOfWeek
                })
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