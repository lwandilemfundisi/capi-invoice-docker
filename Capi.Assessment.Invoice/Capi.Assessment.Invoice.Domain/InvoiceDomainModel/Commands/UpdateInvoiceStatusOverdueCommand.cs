using XFrame.Aggregates.Commands;
using XFrame.Aggregates.ExecutionResults;

namespace Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Commands
{
    #region Command

    public class UpdateInvoiceStatusOverdueCommand : Command<Invoice, InvoiceId>
    {
        #region Constructors
        public UpdateInvoiceStatusOverdueCommand(
            InvoiceId id)
            : base(id)
        {
            Status = "overdue";
        }
        #endregion
        #region Properties

        public string Status { get; }

        #endregion
    }

    #endregion

    #region Command Handler

    public class UpdateInvoiceStatusOverdueCommandHandler : CommandHandler<Invoice, InvoiceId, UpdateInvoiceStatusOverdueCommand>
    {
        public override Task<IExecutionResult> ExecuteAsync(
            Invoice aggregate,
            UpdateInvoiceStatusOverdueCommand command,
            CancellationToken cancellationToken)
        {
            aggregate.UpdateStatusOverdue(command.Status);
            return Task.FromResult<IExecutionResult>(ExecutionResult.Success());
        }
    }

    #endregion
}
