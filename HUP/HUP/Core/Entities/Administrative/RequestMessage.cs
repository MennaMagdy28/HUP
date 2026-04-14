using HUP.Core.Entities.Shared;
using HUP.Core.Enums;
using System;

namespace HUP.Core.Entities.Administrative
{
    public class RequestMessage : BaseEntity
    {
        public Guid RequestId { get; set; }
        public SenderRole SenderRole { get; set; }
        public string Message { get; set; }

        public StudentRequest Request { get; set; }
    }
}
