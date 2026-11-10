using XFrame.Aggregates.Events.AggregateEvents;

namespace Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Events
{
    public class CreatedInvoiceEvent(
        string client, 
        string email, 
        decimal amount, 
        string currency, 
        DateTime issuedDate, 
        DateTime dueDate, 
        string status, 
        string description)
        : AggregateEvent<Invoice, InvoiceId>
    {
        #region Properties

        public string Client { get; } = client;
        public string Email { get; } = email;
        public decimal Amount { get; } = amount;
        public string Currency { get; } = currency;
        public DateTime IssuedDate { get; } = issuedDate;
        public DateTime DueDate { get; } = dueDate;
        public string Status { get; } = status;
        public string Description { get; } = description;

        #endregion
    }
}
