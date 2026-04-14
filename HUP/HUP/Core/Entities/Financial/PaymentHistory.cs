using HUP.Core.Entities.Shared;
using HUP.Core.Enums.Financial;
using System;

namespace HUP.Core.Entities.Financial
{
    public class PaymentHistory : BaseEntity
    {
        public Guid PaymentId { get; set; }
        public PaymentHistoryAction Action { get; set; }
        public DateTime Timestamp { get; set; }
        public string Notes { get; set; }

        public Payment Payment { get; set; }
    }
}
