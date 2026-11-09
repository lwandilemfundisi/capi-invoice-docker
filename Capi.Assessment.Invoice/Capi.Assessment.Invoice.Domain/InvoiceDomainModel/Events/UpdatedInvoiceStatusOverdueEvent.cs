using XFrame.Aggregates.Events.AggregateEvents;

namespace Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Events
{
    public class UpdatedInvoiceStatusOverdueEvent(string status) 
        : AggregateEvent<Invoice, InvoiceId>
    {
        public string Status { get; } = status;
    }
}
