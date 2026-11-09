using XFrame.Aggregates.Entities;

namespace Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Entities
{
    public class Reminder : Entity<ReminderId>
    {
        #region Properties

        public string Client { get; set; } = "";
        public string Message { get; set; } = "";
        public DateTimeOffset SentAt { get; set; }
        public string Type { get; set; } = "reminder";
        public InvoiceId InvoiceId { get; set; }

        #endregion
    }
}
