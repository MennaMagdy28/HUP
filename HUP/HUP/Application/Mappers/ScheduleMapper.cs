using HUP.Application.DTOs.AcademicDtos.CourseOffering;
using HUP.Application.DTOs.AcademicDtos.Schedule;
using HUP.Common.Helpers;
using Riok.Mapperly.Abstractions;
using HUP.Core.Entities.Academics;

namespace HUP.Application.Mappers
{
    [Mapper(AllowNullPropertyAssignment = false)]
    public static partial class ScheduleMapper
    {
        public static partial Schedule ToEntity(ScheduleSlotDto dto);
    }
}