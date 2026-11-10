using Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Commands;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Entities;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Queries;
using InvoiceAggregate = Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Invoice;
using InvoiceId = Capi.Assessment.Invoice.Domain.InvoiceDomainModel.InvoiceId;
using XFrame.Aggregates.Commands;
using XFrame.Aggregates.Queries;
using XFrame.Aggregates.ExecutionResults;

namespace Capi.Assessment.Invoice.Domain.Applications;

public class CapiInvoiceApplication(ICommandBus commandBus, IQueryProcessor queryProcessor) : ICapiInvoiceApplication
{
    public Task<IExecutionResult> CreateInvoiceAsync(
        CreateInvoiceApplicationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateInvoiceCommand(
            InvoiceId.New,
            request.Client,
            request.Email,
            request.Amount,
            request.Currency,
            request.IssuedDate,
            request.DueDate,
            request.DueDate < DateTime.Today ? "overdue" : "sent",
            request.Description);

        return commandBus.PublishAsync(command, cancellationToken);
    }

    public Task<IEnumerable<InvoiceAggregate>> GetInvoicesAsync(string status, DateTime today, CancellationToken cancellationToken)
    {
        var query = new GetInvoicesQuery([status], today);
        return queryProcessor.ProcessAsync(query, cancellationToken);
    }

    public Task<IExecutionResult> MarkInvoicePaidAsync(string invoiceId, CancellationToken cancellationToken)
    {
        var command = new UpdateInvoiceStatusPaidCommand(new InvoiceId(invoiceId));
        return commandBus.PublishAsync(command, cancellationToken);
    }

    public Task<IEnumerable<Reminder>> GetRemindersAsync(CancellationToken cancellationToken)
    {
        var query = new GetNotificationsQuery();
        return queryProcessor.ProcessAsync(query, cancellationToken);
    }

    public async Task<SimulateRemindersResult> SimulateRemindersAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.Today;
        var overdueInvoices = await queryProcessor.ProcessAsync(new GetInvoicesQuery(["paid"], today), cancellationToken);
        if (overdueInvoices is null)
        {
            return new SimulateRemindersResult("Could not retrieve invoices.", 0, 0, null);
        }

        var notifications = await queryProcessor.ProcessAsync(new GetNotificationsQuery(), cancellationToken);
        var sentCount = 0;

        foreach (var invoice in overdueInvoices)
        {
            var daysOverdue = today.Day - invoice.DueDate.Day;
            var reminderAlreadySentToday = notifications?.Any(notification =>
                notification.InvoiceId == invoice.Id
                && notification.SentAt.LocalDateTime == today) ?? false;

            var overdueResult = await commandBus.PublishAsync(
                new UpdateInvoiceStatusOverdueCommand(invoice.Id),
                cancellationToken);
            if (!overdueResult.IsSuccess)
            {
                return new SimulateRemindersResult(
                    $"Could not update invoice {invoice.Id} to overdue status.", 0, 0, null);
            }

            if (reminderAlreadySentToday || daysOverdue < 3)
            {
                continue;
            }

            var reminder = new CreateInvoiceReminderCommand(
                invoice.Id,
                invoice.Client,
                $"Friendly reminder: {invoice.Id} for {invoice.Currency} {invoice.Amount:N2} is {daysOverdue} days overdue.",
                DateTimeOffset.Now,
                "reminder");

            var reminderResult = await commandBus.PublishAsync(reminder, cancellationToken);
            if (reminderResult.IsSuccess)
            {
                sentCount++;
            }
        }

        var checkedCount = overdueInvoices.Count();
        var invoiceNoun = checkedCount == 1 ? "invoice" : "invoices";
        var reminderNoun = sentCount == 1 ? "reminder" : "reminders";
        var message = $"Checked {checkedCount} overdue {invoiceNoun}; simulated {sentCount} {reminderNoun}.";

        return new SimulateRemindersResult(null, checkedCount, sentCount, message);
    }
}
