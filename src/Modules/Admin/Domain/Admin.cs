using HUP.BuildingBlocks.Domain;

namespace HUP.Modules.Admin.Domain
{
    public class Admin : BaseEntity
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime PasswordExpiration { get; set; }
        public string FullName { get; set; }
        public string NationalId {get; set;}
        public bool IsActive { get; set; } = true;
        public AdminRole Role { get; set; }
        public Guid? FacultyId { get; set; }
        public Guid? DepartmentId { get; set; }
    }
}