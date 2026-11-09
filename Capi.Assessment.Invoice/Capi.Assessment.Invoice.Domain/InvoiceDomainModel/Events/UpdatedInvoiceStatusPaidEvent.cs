using XFrame.Aggregates.Events.AggregateEvents;

namespace Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Events
{
    public class UpdatedInvoiceStatusPaidEvent(
        string status, 
        DateTimeOffset paidAt) 
        : AggregateEvent<Invoice, InvoiceId>
    {
        #region Properties

        public string Status { get; } = status;
        public DateTimeOffset? PaidAt { get; } = paidAt;

        #endregion
    }
}
