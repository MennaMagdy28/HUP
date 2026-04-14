using HUP.Core.Entities.Shared;
using System;

namespace HUP.Core.Entities.Administrative
{
    public class RequestDocument : BaseEntity
    {
        public Guid RequestId { get; set; }
        public string FileUrl { get; set; }
        public string DocumentType { get; set; }
        public Guid UploadedBy { get; set; }

        public StudentRequest Request { get; set; }
    }
}
