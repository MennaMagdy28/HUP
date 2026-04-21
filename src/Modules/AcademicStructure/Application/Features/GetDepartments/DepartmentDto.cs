namespace HUP.Modules.AcademicStructure.Application.Features.GetDepartments
{
    public class DepartmentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid FacultyId { get; set; }
        public bool IsProgram { get; set; }
    }
}
