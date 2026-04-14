using HUP.Core.Entities.Shared;
using HUP.Core.Entities.Academics;
using HUP.Core.Enums;
using System;
using System.Collections.Generic;

namespace HUP.Core.Entities.Administrative
{
    public class StudentRequest : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Guid RequestTypeId { get; set; }
        public RequestStatus Status { get; set; }
        public int CurrentStep { get; set; }

        public Student Student { get; set; }
        public RequestType RequestType { get; set; }

        public ICollection<RequestMessage> Messages { get; set; }
        public ICollection<RequestDocument> Documents { get; set; }
    }
}
