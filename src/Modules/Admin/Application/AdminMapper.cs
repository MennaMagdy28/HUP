using Riok.Mapperly.Abstractions;
using HUP.Modules.Admin.Application.Features.GetAdmins;
namespace HUP.Modules.Admin.Application
{
    [Mapper]
    public partial class AdminMapper
    {
        public partial AdminDto AdminToAdminDto(Domain.Admin admin);
        public string MapAdminRole(Domain.AdminRole role) => role.ToString();
    }
}
