namespace HUP.Modules.Privileges.Application.Features.GetRolePermissions
{
    public class RolePermissionsDto
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
    }
}
