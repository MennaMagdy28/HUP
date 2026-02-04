using HUP.Application.DTOs.AcademicDtos;
using HUP.Core.Entities.Academics;
using Riok.Mapperly.Abstractions;
using HUP.Application.DTOs.AcademicDtos.Enrollment;
using HUP.Common.Helpers;

namespace HUP.Application.Mappers
{
    [Mapper(AllowNullPropertyAssignment = false)]
    public static partial class EnrollmentMapper
    {
        // Mapping for CreateEnrollmentDto
        public static partial Enrollment ToEntityFromCreateDto(CreateEnrollmentDto dto);
        public static partial CreateEnrollmentDto ToCreateDto(Enrollment entity);

        // Mapping for EnrollmentResponseDto
        public static partial Enrollment ToEntityFromResponseDto(EnrollmentResponseDto dto);

        public static EnrollmentResponseDto ToResponseDto(Enrollment entity, string lang)
        {
            var dto = new EnrollmentResponseDto();
            dto.Id = entity.Id;
            dto.EnrollmentDate = entity.EnrollmentDate;
            dto.Status = LocalizationHelper.Get(entity.Status, lang);
            dto.Grade = entity.finalGrade;
            dto.CourseCode = LocalizationHelper.Get<string>(entity.CourseOffering.Course.CourseCode, lang);
            dto.CourseCode = LocalizationHelper.Get<string>(entity.CourseOffering.Course.CourseName, lang);
            return dto;
        }

        //Mapping for UpdateEnrollmentDto
        // status update
        public static partial Enrollment ToEntityFromUpdateDto(UpdateEnrollmentDto dto);
        public static partial void ToUpdate(UpdateEnrollmentDto dto, Enrollment entity);

        // grades update
        public static partial Enrollment ToEntityFromUpdateGradeDto(UpdateEnrollmentDto dto);

    }
}
