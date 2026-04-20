using HUP.BuildingBlocks.Application;
namespace HUP.Modules.Privileges.Application.Features.GetRolePermissions
{
    public class GetRolePermissionsHandler
    {
        public async Task<Result<RolePermissionsDto>> HandleAsync(GetRolePermissionsQuery request, CancellationToken cancellationToken)
        {
            return Result<RolePermissionsDto>.Success(new RolePermissionsDto());
        }
    }
}
