using XFrame.Aggregates.Events.AggregateEvents;

namespace Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Events
{
    public class CreatedInvoiceReminderEvent(
        string client, 
        string message, 
        DateTimeOffset sentAt, 
        string type) 
        : AggregateEvent<Invoice, InvoiceId>
    {
        #region Properties

        public string Client { get; } = client;
        public string Message { get; } = message;
        public DateTimeOffset SentAt { get; } = sentAt;
        public string Type { get; } = type;

        #endregion
    }
}
