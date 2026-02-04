using HUP.Application.DTOs.AcademicDtos.Schedule;

namespace HUP.Application.Services.Interfaces;

public interface IScheduleService
{
    Task Create(ScheduleSlotDto dto);
    Task Update(ScheduleSlotDto dto);
    Task SoftDelete(Guid id);
    Task Remove(Guid dto);
    
}