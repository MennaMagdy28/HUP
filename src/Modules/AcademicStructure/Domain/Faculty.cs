using HUP.BuildingBlocks.Domain;
namespace HUP.Modules.AcademicStructure.Domain
{
    public class Faculty : BaseEntity
    {
        public string Name { get; set; }
        public Guid UniversityId { get; set; }
        public University University { get; set; }
        public ICollection<Department> Departments { get; set; } = new List<Department>();
    }
}
