using HUP.BuildingBlocks.Domain;

namespace HUP.Modules.Admin.Domain
{
    public class AdminRole : BaseEntity
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string? Description { get; set; }
        public Guid? CreatedBy { get; set; }
    }
}