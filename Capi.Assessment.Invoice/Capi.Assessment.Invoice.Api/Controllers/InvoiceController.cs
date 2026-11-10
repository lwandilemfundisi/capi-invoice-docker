using Capi.Assessment.Invoice.Api.Extensions;
using Capi.Assessment.Invoice.Api.Mappers.Abstract;
using Capi.Assessment.Invoice.Api.Models.RequestModels;
using Capi.Assessment.Invoice.Api.Models.ResponseModels;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Commands;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Queries;
using Microsoft.AspNetCore.Mvc;
using XFrame.Aggregates.Commands;
using XFrame.Aggregates.Queries;
using XFrame.Common.Extensions;

namespace Capi.Assessment.Invoice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoiceController(
    ICommandBus commandBus,
    IQueryProcessor queryProcessor,
    IMappingRegistry mappingRegistry) : ControllerBase
{
    [HttpPost("create")]
    public async Task<ActionResult<CreateInvoiceResponseModel>> CreateInvoice(
        CreateInvoiceRequestModel model,
        CancellationToken cancellationToken)
    {
        var command = new CreateInvoiceCommand(
            InvoiceId.New,
            model.Client,
            model.Email,
            model.Amount,
            model.Currency,
            model.IssuedDate,
            model.DueDate,
            model.DueDate < DateTime.Today
                ? "overdue"
                : "sent",
            model.Description);

        var result = await commandBus.PublishAsync(command, cancellationToken);

        var response = mappingRegistry.Map<CreateInvoiceResponseModel>(result.GetResult<Domain.InvoiceDomainModel.Invoice>());

        return result.IsSuccess
            ? Ok(response)
            : UnprocessableEntity(response);
    }

    [HttpGet("list")]
    public async Task<ActionResult<GetInvoicesResponseModel>> ListInvoices(
        [FromQuery] GetInvoicesRequestModel model,
        CancellationToken cancellationToken)
    {
        var query = new GetInvoicesQuery([model.Status], model.TodaysDate);
        var result = await queryProcessor.ProcessAsync(query, cancellationToken);
        var response = mappingRegistry.Map<GetInvoicesResponseModel>(result);

        return Ok(response);
    }

    [HttpPatch("{id}/paid")]
    public async Task<ActionResult<UpdateInvoiceStatusPaidResponseModel>> UpdateInvoiceStatus(
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        var command = new UpdateInvoiceStatusPaidCommand(new InvoiceId(id));
        var result = await commandBus.PublishAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : UnprocessableEntity(result);
    }

    [HttpGet("reminders")]
    public async Task<ActionResult<GetRemindersResponseModel>> GetNotifications(
        CancellationToken cancellationToken)
    {
        var query = new GetNotificationsQuery();
        var result = await queryProcessor.ProcessAsync(query, cancellationToken);
        var response = mappingRegistry.Map<GetRemindersResponseModel>(result);

        return Ok(response);
    }

    [HttpPost("reminders/simulate")]
    public async Task<ActionResult<SimulateReminderResponseModel>> SimulateReminder(
        CancellationToken cancellationToken)
    {
        var today = DateTime.Today;
        var invoicesQuery = new GetInvoicesQuery(["paid"], today);
        var invoicesResult = await queryProcessor.ProcessAsync(invoicesQuery, cancellationToken);
        var invoiceResponse = mappingRegistry.Map<GetInvoicesResponseModel>(invoicesResult);

        if (invoiceResponse?.Result is null)
        {
            return Problem("Could not retrieve invoices.");
        }

        var overdueInvoices = invoiceResponse.Result;

        var notificationsQuery = new GetNotificationsQuery();
        var notificationsResult = await queryProcessor.ProcessAsync(notificationsQuery, cancellationToken);
        var notificationResponse = mappingRegistry.Map<GetRemindersResponseModel>(notificationsResult);
        var notifications = notificationResponse?.Result;
        var sentCount = 0;

        foreach (var invoice in overdueInvoices)
        {
            var daysOverdue = today.Day - invoice.DueDate.Day;
            var reminderAlreadySentToday = notifications?.Any(notification =>
                notification.InvoiceId == invoice.Id
                && notification.SentAt.LocalDateTime == today) ?? false;

            var overdueCmd = new UpdateInvoiceStatusOverdueCommand(new InvoiceId(invoice.Id));
            var overdueCmdResult = await commandBus.PublishAsync(overdueCmd, cancellationToken);

            if (!overdueCmdResult.IsSuccess)
            {
                return Problem($"Could not update invoice {invoice.Id} to overdue status.");
            }

            if (!ShouldSendReminder(daysOverdue, reminderAlreadySentToday))
            {
                continue;
            }

            var reminderCmd = new CreateInvoiceReminderCommand(
                new InvoiceId(invoice.Id),
                invoice.Client,
                $"Friendly reminder: {invoice.Id} for {invoice.Currency} " +
                $"{invoice.Amount:N2} is {daysOverdue} days overdue.",
                DateTimeOffset.Now,
                "reminder");

            var reminderCmdResult = await commandBus.PublishAsync(reminderCmd, cancellationToken);
            if (reminderCmdResult.IsSuccess)
            {
                sentCount++;
            }
        }

        var checkedCount = overdueInvoices.Count;
        var invoiceNoun = checkedCount == 1 ? "invoice" : "invoices";
        var reminderNoun = sentCount == 1 ? "reminder" : "reminders";

        return Ok(new SimulateReminderResponseModel
        {
            CheckedCount = checkedCount,
            SentCount = sentCount,
            OverdueCount = checkedCount,
            Message = $"Checked {checkedCount} overdue {invoiceNoun}; " +
                      $"simulated {sentCount} {reminderNoun}."
        });
    }

    private static bool ShouldSendReminder(int daysOverdue, bool alreadySentToday)
    {
        return !alreadySentToday
            && daysOverdue >= 3
            ;
    }
}
