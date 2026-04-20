namespace HUP.Modules.AcademicStructure.Application.Features.AddLevels
{
    public class AddLevelsQuery
    {
        public string Name { get; set; }
        public int AcademicYear { get; set; }
        public Guid DepartmentId { get; set; }
    }
}
