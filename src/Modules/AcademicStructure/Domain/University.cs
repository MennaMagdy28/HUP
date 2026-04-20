using HUP.BuildingBlocks.Domain;
namespace HUP.Modules.AcademicStructure.Domain
{
    public class University : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public ICollection<Faculty> Faculties { get; set; } = new List<Faculty>();
    }
}
