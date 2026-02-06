using HUP.Application.DTOs.AcademicDtos.Enrollment;
using HUP.Core.Entities.Academics;

namespace HUP.Application.Services.Interfaces
{
    public interface IEnrollmentService
    {
        //CRUD queries + soft delete
        Task<EnrollmentResponseDto?> GetByIdAsync(Guid id, string lang);
        Task<IEnumerable<EnrollmentResponseDto>> GetAllAsync(string lang);
        Task AddAsync(CreateEnrollmentDto createEnrollmentDto);
        Task Update(Guid id, UpdateEnrollmentDto updateEnrollmentDto);

        Task SoftDelete(Guid id);
        Task<bool> Exists(CreateEnrollmentDto dto);
        Task Remove(Guid id);
        Task<List<SemesterTranscriptDto>> GetStudentGradesAsync(Guid studentId, string lang);
        Task<IEnumerable<EnrollmentResponseDto>> GetRegisteredByStudentAsync(Guid studentId, string lang, EnrollmentFilterDto filter);
    }
}
