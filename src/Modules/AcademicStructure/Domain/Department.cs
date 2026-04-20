using HUP.BuildingBlocks.Domain;
namespace HUP.Modules.AcademicStructure.Domain
{
    public class Department : BaseEntity
    {
        public string Name { get; set; }
        public Guid FacultyId { get; set; }
        public Faculty Faculty { get; set; }
        public bool IsProgram { get; set; }
        public ICollection<Levels> Levels { get; set; } = new List<Levels>();
    }
}
