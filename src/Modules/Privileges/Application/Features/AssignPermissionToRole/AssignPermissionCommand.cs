namespace HUP.Modules.Privileges.Application.Features.AssignPermissionToRole
{
    public class AssignPermissionCommand
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
    }
}
