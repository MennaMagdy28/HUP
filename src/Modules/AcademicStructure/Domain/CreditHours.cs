using HUP.BuildingBlocks.Domain;

namespace HUP.Modules.AcademicStructure.Domain
{
    public class CreditHours : BaseEntity
    {
        public int TotalCreditsRequired { get; set; }
        public int CompulsoryCreditsRequired { get; set; }
        public int ElectiveCreditsRequired { get; set; }
        public Guid DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}