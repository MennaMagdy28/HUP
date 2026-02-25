using HUP.Application.DTOs.AcademicDtos.Enrollment;

namespace HUP.Application.Validators.Interfaces
{
    public interface IEnrollmentValidator
    {
        Task ValidateEnrollmentAsync(CreateEnrollmentDto dto);
    }
}
