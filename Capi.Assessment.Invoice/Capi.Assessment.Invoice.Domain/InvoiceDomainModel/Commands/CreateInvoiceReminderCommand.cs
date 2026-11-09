using XFrame.Aggregates.Commands;
using XFrame.Aggregates.ExecutionResults;

namespace Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Commands
{
    #region Command

    public class CreateInvoiceReminderCommand : Command<Invoice, InvoiceId>
    {
        public CreateInvoiceReminderCommand(
            InvoiceId aggregateId,
            string client,
            string message,
            DateTimeOffset sentAt,
            string type)
            : base(aggregateId)
        {
            Client = client;
            Message = message;
            SentAt = sentAt;
            Type = type;
        }

        #region Properties

        public string Client { get; }
        public string Message { get; }
        public DateTimeOffset SentAt { get; }
        public string Type { get; }

        #endregion
    }

    #endregion

    #region Command Handler

    public class CreateInvoiceReminderCommandHandler : CommandHandler<Invoice, InvoiceId, CreateInvoiceReminderCommand>
    {
        public override Task<IExecutionResult> ExecuteAsync(
            Invoice aggregate, 
            CreateInvoiceReminderCommand command, 
            CancellationToken cancellationToken)
        {
            aggregate.CreateReminder(command.Client, command.Message, command.SentAt, command.Type);
            return Task.FromResult(ExecutionResult.Success());
        }
    }

    #endregion
}
