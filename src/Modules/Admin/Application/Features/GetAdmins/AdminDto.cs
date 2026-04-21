namespace HUP.Modules.Admin.Application.Features.GetAdmins
{
    public class AdminDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public Guid? FacultyId { get; set; }
        public Guid? DepartmentId { get; set; }
    }
}
