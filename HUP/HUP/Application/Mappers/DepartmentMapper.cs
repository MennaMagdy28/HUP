using HUP.Application.DTOs.AcademicDtos;
using HUP.Common.Helpers;
using HUP.Core.Entities.Academics;
using Riok.Mapperly.Abstractions;

namespace HUP.Application.Mappers
{
    [Mapper(AllowNullPropertyAssignment = false)]
    public static partial class DepartmentMapper
    {
        public static DepartmentDto ToDto(Department entity, string lang)
        {
            var dto = new DepartmentDto();
            dto.Id = entity.Id;
            dto.Name = LocalizationHelper.Get<string>(entity.DepartmentName, lang);
            dto.Code = entity.DepartmentCode;
            return dto;
        }

        public static partial List<DepartmentDto> ToDtoList(IEnumerable<Department> entities, string lang);
    }
}