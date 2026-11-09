using XFrame.Aggregates.Commands;
using XFrame.Aggregates.ExecutionResults;

namespace Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Commands
{
    #region Command

    public class UpdateInvoiceStatusPaidCommand : Command<Invoice, InvoiceId>
    {
        #region Constructors
        public UpdateInvoiceStatusPaidCommand(
            InvoiceId id)
            : base(id)
        {
            Status = "paid";
            PaidAt = DateTimeOffset.Now;
        }
        #endregion
        #region Properties

        public DateTimeOffset PaidAt { get; }
        public string Status { get; }

        #endregion
    }

    #endregion

    #region Command Handler

    public class UpdateInvoiceStatusPaidCommandHandler : CommandHandler<Invoice, InvoiceId, UpdateInvoiceStatusPaidCommand>
    {
        public override Task<IExecutionResult> ExecuteAsync(
            Invoice aggregate, 
            UpdateInvoiceStatusPaidCommand command, 
            CancellationToken cancellationToken)
        {
            aggregate.UpdateStatusPaid(command.Status, command.PaidAt);
            return Task.FromResult<IExecutionResult>(ExecutionResult.Success());
        }
    }

    #endregion
}
