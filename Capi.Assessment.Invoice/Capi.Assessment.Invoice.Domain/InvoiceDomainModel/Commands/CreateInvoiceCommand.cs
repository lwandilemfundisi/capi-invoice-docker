using Capi.Assessment.Invoice.Domain.Extensions;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Rules;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Rules.Contexts;
using XFrame.Aggregates.Commands;
using XFrame.Aggregates.ExecutionResults;
using XFrame.Common;
using XFrame.Notifications.Extensions;
using XFrame.Rules;

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
            DateTime issuedDate,
            DateTime dueDate,
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
        public DateTime IssuedDate { get; }
        public DateTime DueDate { get; }
        public string Status { get; }
        public string Description { get; }
        public DateTimeOffset? PaidAt { get; }

        #endregion
    }

    #endregion

    #region Command Handler

    public class CreateInvoiceCommandHandler(IValidator validator) : CommandHandler<Invoice, InvoiceId, CreateInvoiceCommand>
    {
        #region Methods

        public async override Task<IExecutionResult> ExecuteAsync(
            Invoice aggregate, 
            CreateInvoiceCommand command, 
            CancellationToken cancellationToken)
        {
            var notifications = await validator.ValidateAsync<CreateInvoiceCommand, InvoiceValidationContext>(command, cancellationToken);

            if (notifications.HasErrors())
            {
                return ExecutionResult.Failed(notifications);
            }

            aggregate.CreateInvoice(
                command.Client,
                command.Email,
                command.Amount,
                command.Currency,
                command.IssuedDate,
                command.DueDate,
                command.Status,
                command.Description);

            return ExecutionResult.Success(aggregate);
        }

        #endregion
    }

    #endregion


}
