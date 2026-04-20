namespace HUP.Modules.Students.Application.Features.CreateStudent
{
    public class CreateStudentQuery
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string UniversityCode { get; set; }
        public Guid DepartmentId { get; set; }
        public int Level { get; set; }
    }
}
