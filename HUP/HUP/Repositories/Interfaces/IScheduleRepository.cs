using HUP.Core.Entities.Academics;
using HUP.Core.Models;

namespace HUP.Repositories.Interfaces;

public interface IScheduleRepository : IGenericRepository<HUP.Core.Entities.Academics.Schedule>
{
    Task<IEnumerable<ScheduleSlot>> GetByStudentEnrollmentsAsync(Guid studentId);
    Task<IEnumerable<ScheduleSlot>> GetAvailableSlotsAsync();
    Task<bool> TryBookSeatAsync(Guid scheduleId);
    Task<Schedule> GetByIdWithDetailsAsync(Guid id);
}