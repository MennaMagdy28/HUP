using HUP.Core.Entities.Shared;
using HUP.Core.Enums.Financial;
using System;
using System.Collections.Generic;

namespace HUP.Core.Entities.Financial
{
    public class Payment : BaseEntity
    {
        public Guid InvoiceId { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod Method { get; set; }
        public string ReferenceNumber { get; set; }

        public Invoice Invoice { get; set; }
        public ICollection<PaymentHistory> History { get; set; }
    }
}
