namespace HUP.Core.Enums.Financial
{
    public enum InvoiceStatus
    {
        Unpaid,
        PartiallyPaid,
        Paid,
        Overdue
    }

    public enum PaymentHistoryAction
    {
        Created,
        Updated,
        Refunded
    }
}
