using HUP.Core.Entities.Shared;
using System;

namespace HUP.Core.Entities.Financial
{
    public class InvoiceItem : BaseEntity
    {
        public Guid InvoiceId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }

        public Invoice Invoice { get; set; }
    }
}
