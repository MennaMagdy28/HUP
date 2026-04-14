using HUP.Core.Entities.Shared;
using System.Collections.Generic;

namespace HUP.Core.Entities.Administrative
{
    public class RequestType : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<StudentRequest> Requests { get; set; }
        public ICollection<RequestTypeScope> Scopes { get; set; }
    }
}
