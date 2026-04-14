using HUP.Core.Entities.Shared;
using HUP.Core.Entities.Academics;
using System;

namespace HUP.Core.Entities.Administrative
{
    public class RequestTypeScope : BaseEntity
    {
        public Guid RequestTypeId { get; set; }
        public Guid? FacultyId { get; set; }
        public Guid? DepartmentId { get; set; }
        public bool IsActive { get; set; }

        public RequestType RequestType { get; set; }
        public Faculty Faculty { get; set; }
        public Department Department { get; set; }
    }
}
