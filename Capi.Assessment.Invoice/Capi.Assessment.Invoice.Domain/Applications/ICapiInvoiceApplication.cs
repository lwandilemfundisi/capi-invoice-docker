using XFrame.Aggregates.ExecutionResults;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Entities;
using InvoiceAggregate = Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Invoice;
using Capi.Assessment.Invoice.Domain.Applications.Models;

namespace Capi.Assessment.Invoice.Domain.Applications;

public interface ICapiInvoiceApplication
{
    Task<IExecutionResult> CreateInvoiceAsync(string client, string email, decimal amount, string currency, DateTime issuedDate, DateTime dueDate, string description, CancellationToken cancellationToken);
    Task<IEnumerable<InvoiceAggregate>> GetInvoicesAsync(string status, DateTime today, CancellationToken cancellationToken);
    Task<IExecutionResult> MarkInvoicePaidAsync(string invoiceId, CancellationToken cancellationToken);
    Task<IEnumerable<Reminder>> GetRemindersAsync(CancellationToken cancellationToken);
    Task<SimulateRemindersResult> SimulateRemindersAsync(CancellationToken cancellationToken);
}


