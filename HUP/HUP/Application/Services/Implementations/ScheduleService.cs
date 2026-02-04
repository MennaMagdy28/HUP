using HUP.Application.DTOs.AcademicDtos.Schedule;
using HUP.Application.Mappers;
using HUP.Application.Services.Interfaces;
using HUP.Core.Entities.Academics;
using HUP.Repositories.Interfaces;

namespace HUP.Application.Services.Implementations;

public class ScheduleService : IScheduleService
{
    private readonly IScheduleRepository _repository;

    public ScheduleService(IScheduleRepository scheduleRepository)
    {
        _repository = scheduleRepository;
    }
    public async Task Create(ScheduleSlotCreateDto createDto)
    {
        var slot = new Schedule();
        slot.Id = Guid.NewGuid();
        slot.CreatedAt = DateTime.Now;
        slot = ScheduleMapper.ToEntity(createDto);
        await _repository.AddAsync(slot);
    }

    public Task Update(ScheduleSlotCreateDto createDto)
    {
        throw new NotImplementedException();
    }

    public async Task SoftDelete(Guid id)
    {
        var entity = await _repository.GetByIdTracking(id);
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.Now;
        await _repository.SaveChangesAsync();
    }

    public Task Remove(Guid dto)
    {
        throw new NotImplementedException();
    }
}