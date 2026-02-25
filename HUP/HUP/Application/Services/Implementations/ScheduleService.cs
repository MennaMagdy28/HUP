using HUP.Application.DTOs.AcademicDtos.Schedule;
using HUP.Application.Mappers;
using HUP.Application.Services.Interfaces;
using HUP.Core.Entities.Academics;
using HUP.Repositories.Interfaces;

namespace HUP.Application.Services.Implementations
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _repository;

        public ScheduleService(IScheduleRepository scheduleRepository)
        {
            _repository = scheduleRepository;
        }

        public async Task Create(ScheduleSlotCreateDto createDto)
        {
            var slot = ScheduleMapper.ToEntity(createDto);
            slot.Id = Guid.NewGuid();
            slot.CreatedAt = DateTime.Now;
            slot.AvailableSeats = createDto.TotalSeats; // Initialize available seats
            await _repository.AddAsync(slot);
        }

        public async Task<IEnumerable<ScheduleSlotDto>> GetSlotsByStudentEnrollments(Guid studentId, string lang)
        {
            var slots = await _repository.GetByStudentEnrollmentsAsync(studentId);
            return slots.Select(s => ScheduleMapper.ToDto(s, lang));
        }

        public async Task<IEnumerable<ScheduleSlotDto>> GetAvailableSlotsForEnrollment(string lang)
        {
            var slots = await _repository.GetAvailableSlotsAsync();
            return slots.Select(s => ScheduleMapper.ToDto(s, lang));
        }

        public Task Update(ScheduleSlotCreateDto createDto)
        {
            throw new NotImplementedException();
        }

        public async Task SoftDelete(Guid id)
        {
            var entity = await _repository.GetByIdTracking(id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                entity.UpdatedAt = DateTime.Now;
                await _repository.SaveChangesAsync();
            }
        }

        public async Task Remove(Guid id)
        {
             await _repository.RemoveAsync(id);
        }
    }
}