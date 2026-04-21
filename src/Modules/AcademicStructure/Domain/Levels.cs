using HUP.BuildingBlocks.Domain;
namespace HUP.Modules.AcademicStructure.Domain
{
    public class Levels : BaseEntity
    {
        public string Name { get; set; }
        public int AcademicYear { get; set; }
        public Guid DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
