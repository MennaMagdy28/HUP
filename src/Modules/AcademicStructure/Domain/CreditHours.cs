using HUP.BuildingBlocks.Domain;
namespace HUP.Modules.AcademicStructure.Domain
{
    public class CreditHours : BaseEntity
    {
        public int TotalCreditsRequired { get; set; }
        public Guid DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
