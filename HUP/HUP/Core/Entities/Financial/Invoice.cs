using HUP.Core.Entities.Shared;
using HUP.Core.Entities.Academics;
using HUP.Core.Enums.Financial;
using System;
using System.Collections.Generic;

namespace HUP.Core.Entities.Financial
{
    public class Invoice : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Guid SemesterId { get; set; }
        public decimal TotalAmount { get; set; }
        public InvoiceStatus Status { get; set; }

        public Student Student { get; set; }
        public Semester Semester { get; set; }

        public ICollection<InvoiceItem> Items { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}
