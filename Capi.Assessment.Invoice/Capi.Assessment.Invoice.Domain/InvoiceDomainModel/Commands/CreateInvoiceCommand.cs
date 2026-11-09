using XFrame.Aggregates.Commands;
using XFrame.Aggregates.ExecutionResults;

namespace Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Commands
{
    #region Command

    public class CreateInvoiceCommand : Command<Invoice, InvoiceId>
    {
        #region Constructors

        public CreateInvoiceCommand(
            InvoiceId id,
            string client,
            string email,
            decimal amount,
            string currency,
            DateOnly issuedDate,
            DateOnly dueDate,
            string status,
            string description)
            : base(id)
        {
            Client = client;
            Email = email;
            Amount = amount;
            Currency = currency;
            IssuedDate = issuedDate;
            DueDate = dueDate;
            Status = status;
            Description = description;
        }

        #endregion

        #region Properties

        public string Client { get; }
        public string Email { get; }
        public decimal Amount { get; }
        public string Currency { get; }
        public DateOnly IssuedDate { get; }
        public DateOnly DueDate { get; }
        public string Status { get; }
        public string Description { get; }
        public DateTimeOffset? PaidAt { get; }

        #endregion
    }

    #endregion

    #region Command Handler

    public class CreateInvoiceCommandHandler : CommandHandler<Invoice, InvoiceId, CreateInvoiceCommand>
    {
        #region Methods

        public override Task<IExecutionResult> ExecuteAsync(
            Invoice aggregate, 
            CreateInvoiceCommand command, 
            CancellationToken cancellationToken)
        {
            aggregate.CreateInvoice(
                command.Client,
                command.Email,
                command.Amount,
                command.Currency,
                command.IssuedDate,
                command.DueDate,
                command.Status,
                command.Description);

            return Task.FromResult(ExecutionResult.Success(aggregate));
        }

        #endregion
    }

    #endregion


}
