using HUP.BuildingBlocks.Domain;
namespace HUP.Modules.Privileges.Domain
{
    public class Role : BaseEntity
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string? Description { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
