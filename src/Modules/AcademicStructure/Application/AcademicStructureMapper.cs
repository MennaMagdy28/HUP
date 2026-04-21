using Riok.Mapperly.Abstractions;
using HUP.Modules.AcademicStructure.Domain;
using HUP.Modules.AcademicStructure.Application.Features.GetFaculties;
using HUP.Modules.AcademicStructure.Application.Features.GetDepartments;
using HUP.Modules.AcademicStructure.Application.Features.GetLevels;
namespace HUP.Modules.AcademicStructure.Application
{
    [Mapper]
    public partial class AcademicStructureMapper
    {
        public partial FacultyDto FacultyToFacultyDto(Faculty faculty);
        public partial DepartmentDto DepartmentToDepartmentDto(Department department);
        public partial LevelsDto LevelsToLevelsDto(Levels levels);
    }
}
