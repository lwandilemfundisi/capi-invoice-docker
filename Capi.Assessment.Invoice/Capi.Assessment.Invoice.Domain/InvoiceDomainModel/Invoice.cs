using Capi.Assessment.Invoice.Domain.Extensions;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Entities;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Events;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Specifications;
using XFrame.Aggregates;
using XFrame.Common.Extensions;
using XFrame.Specifications.Extensions;

namespace Capi.Assessment.Invoice.Domain.InvoiceDomainModel
{
    public class Invoice : AggregateRoot<Invoice, InvoiceId>
    {
        private ICollection<Reminder> _reminders;

        #region Constructors

        public Invoice()
            : base(null)
        {
        }

        public Invoice(InvoiceId id)
            : base(id)
        {

        }

        private Invoice(Action<object, string> lazyLoader)
            : base(null)
        {
            LazyLoader = lazyLoader;
        }

        #endregion

        #region Properties

        private Action<object, string> LazyLoader { get; set; }

        public ICollection<Reminder> Reminders
        {
            get => LazyLoader.Load(this, ref _reminders);
            set => _reminders = value;
        }

        public string Client { get; set; }
        public string Email { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public DateTimeOffset? PaidAt { get; set; }

        #endregion

        #region Methods

        public void CreateInvoice(
            string client, 
            string email, 
            decimal amount, 
            string currency, 
            DateTime issuedDate, 
            DateTime dueDate, 
            string status, 
            string description)
        {
            AggregateSpecifications
                .AggregateIsNew
                .ThrowDomainErrorIfNotSatisfied(this);

            Client = client;
            Email = email;
            Amount = amount;
            Currency = currency;
            IssuedDate = issuedDate;
            DueDate = dueDate;
            Status = status;
            Description = description;

            Emit(new CreatedInvoiceEvent(
                client,
                email,
                amount,
                currency,
                issuedDate,
                dueDate,
                status,
                description));
        }

        internal void UpdateStatusPaid(
            string status, 
            DateTimeOffset paidAt)
        {
            AggregateSpecifications.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);

            Status = status;
            PaidAt = paidAt;

            Emit(new UpdatedInvoiceStatusPaidEvent(
                status,
                paidAt));
        }

        public void CreateReminder(
            string client, 
            string message, 
            DateTimeOffset sentAt, 
            string type)
        {
            AggregateSpecifications.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);

            var reminder = new Reminder
            {
                Id = ReminderId.New,
                Client = client,
                Message = message,
                SentAt = sentAt,
                Type = type
            };

            Reminders.Add(reminder);

            Emit(new CreatedInvoiceReminderEvent(
                client, 
                message, 
                sentAt, 
                type));
        }

        public void UpdateStatusOverdue(string status)
        {
            AggregateSpecifications.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);

            Status = status;
            Emit(new UpdatedInvoiceStatusOverdueEvent(status));
        }

        #endregion
    }
}
