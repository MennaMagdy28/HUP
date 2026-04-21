using HUP.BuildingBlocks.Domain;
using HUP.Modules.AcademicStructure.Enums;


namespace HUP.Modules.AcademicStructure.Domain
{
    public class Faculty : BaseEntity
    {
        public FacultyTitle Title { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public Guid DeanId { get; set; } // <<======
        public string DeanName { get; set; } // xx?
        public string ContactInfo { get; set; }
        public Guid UniversityId { get; set; }
        public University University { get; set; }
        public ICollection<Department> Departments { get; set; } = new List<Department>();
    }
}