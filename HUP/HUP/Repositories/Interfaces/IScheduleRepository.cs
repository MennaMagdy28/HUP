using HUP.Core.Entities.Academics;

namespace HUP.Repositories.Interfaces;

public interface IScheduleRepository : IGenericRepository<HUP.Core.Entities.Academics.Schedule>
{
    Task<IEnumerable<HUP.Core.Entities.Academics.Schedule>> GetByStudentEnrollmentsAsync(Guid studentId);
    Task<IEnumerable<Schedule>> GetAvailableSlotsAsync();
    Task<bool> TryBookSeatAsync(Guid scheduleId);
    Task<Schedule> GetByIdWithDetailsAsync(Guid id);
}