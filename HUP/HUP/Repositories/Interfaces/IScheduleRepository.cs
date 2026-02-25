namespace HUP.Repositories.Interfaces;

public interface IScheduleRepository : IGenericRepository<HUP.Core.Entities.Academics.Schedule>
{
    Task<IEnumerable<HUP.Core.Entities.Academics.Schedule>> GetByStudentEnrollmentsAsync(Guid studentId);
    Task<IEnumerable<HUP.Core.Entities.Academics.Schedule>> GetAvailableSlotsAsync();
    Task<bool> TryBookSeatAsync(Guid scheduleId);
}