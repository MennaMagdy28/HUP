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
        public static partial Schedule ToEntity(ScheduleSlotCreateDto createDto);

        public static ScheduleSlotDto ToDto(Schedule entity, string lang)
        {
            var slot = new ScheduleSlotDto();
            slot.CourseCode = LocalizationHelper.Get<string>(entity.CourseOffering.Course.CourseCode, lang);
            slot.CourseName = LocalizationHelper.Get<string>(entity.CourseOffering.Course.CourseName, lang);
            slot.DayOfWeek = LocalizationHelper.Get(entity.DayOfWeek, lang);
            slot.StartTime = entity.StartTime;
            slot.EndTime = entity.EndTime;
            slot.Group = entity.Group;
            slot.InstructorName = LocalizationHelper.Get<string>(entity.InstructorName, lang);
            slot.Hall = LocalizationHelper.Get<string>(entity.Hall, lang);
            return slot;
        }
    }
}