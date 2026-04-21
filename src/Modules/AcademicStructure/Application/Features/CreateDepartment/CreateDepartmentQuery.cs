namespace HUP.Modules.AcademicStructure.Application.Features.CreateDepartment
{
    public class CreateDepartmentQuery
    {
        public string Name { get; set; }
        public Guid FacultyId { get; set; }
        public bool IsProgram { get; set; }
    }
}
