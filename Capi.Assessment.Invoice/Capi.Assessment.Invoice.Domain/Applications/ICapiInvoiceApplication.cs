using XFrame.Aggregates.ExecutionResults;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Entities;
using InvoiceAggregate = Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Invoice;

namespace Capi.Assessment.Invoice.Domain.Applications;

public interface ICapiInvoiceApplication
{
    Task<IExecutionResult> CreateInvoiceAsync(CreateInvoiceApplicationRequest request, CancellationToken cancellationToken);
    Task<IEnumerable<InvoiceAggregate>> GetInvoicesAsync(string status, DateTime today, CancellationToken cancellationToken);
    Task<IExecutionResult> MarkInvoicePaidAsync(string invoiceId, CancellationToken cancellationToken);
    Task<IEnumerable<Reminder>> GetRemindersAsync(CancellationToken cancellationToken);
    Task<SimulateRemindersResult> SimulateRemindersAsync(CancellationToken cancellationToken);
}

public sealed record CreateInvoiceApplicationRequest(
    string Client,
    string Email,
    decimal Amount,
    string Currency,
    DateTime IssuedDate,
    DateTime DueDate,
    string Description);

public sealed record SimulateRemindersResult(string Error, int CheckedCount, int SentCount, string Message);
