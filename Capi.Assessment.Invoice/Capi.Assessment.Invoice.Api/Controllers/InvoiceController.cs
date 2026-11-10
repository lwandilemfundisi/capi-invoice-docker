using Azure;
using Capi.Assessment.Invoice.Api.Extensions;
using Capi.Assessment.Invoice.Api.Mappers.Abstract;
using Capi.Assessment.Invoice.Api.Models.RequestModels;
using Capi.Assessment.Invoice.Api.Models.ResponseModels;
using Capi.Assessment.Invoice.Api.Models.ResponseModels.Abstract;
using Capi.Assessment.Invoice.Domain.Applications;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XFrame.Aggregates.ExecutionResults;

namespace Capi.Assessment.Invoice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoiceController(
    ICapiInvoiceApplication invoiceApplication,
    IMappingRegistry mappingRegistry) : ControllerBase
{
    [HttpPost("create")]
    public async Task<CreateInvoiceResponseModel> CreateInvoice(
        CreateInvoiceRequestModel model,
        CancellationToken cancellationToken)
    {
        var result = await invoiceApplication.CreateInvoiceAsync(
            model.Client,
            model.Email,
            model.Amount,
            model.Currency,
            model.IssuedDate,
            model.DueDate,
            model.Description, 
            cancellationToken);

        return GetResponse<Domain.InvoiceDomainModel.Invoice, CreateInvoiceResponseModel>(result);
    }

    [HttpGet("list")]
    public async Task<ActionResult<GetInvoicesResponseModel>> ListInvoices(
        [FromQuery] GetInvoicesRequestModel model,
        CancellationToken cancellationToken)
    {
        var result = await invoiceApplication.GetInvoicesAsync(model.Status, model.TodaysDate, cancellationToken);
        return Ok(mappingRegistry.Map<GetInvoicesResponseModel>(result));
    }

    [HttpPatch("{id}/paid")]
    public async Task<UpdateInvoiceStatusPaidResponseModel> UpdateInvoiceStatus(
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        var result = await invoiceApplication.MarkInvoicePaidAsync(id, cancellationToken);
        return GetResponse<UpdateInvoiceStatusPaidResponseModel>(result);
    }

    [HttpGet("reminders")]
    public async Task<ActionResult<GetRemindersResponseModel>> GetNotifications(
        CancellationToken cancellationToken)
    {
        var result = await invoiceApplication.GetRemindersAsync(cancellationToken);
        return Ok(mappingRegistry.Map<GetRemindersResponseModel>(result));
    }

    [HttpPost("reminders/simulate")]
    public async Task<ActionResult<SimulateReminderResponseModel>> SimulateReminder(
        CancellationToken cancellationToken)
    {
        var result = await invoiceApplication.SimulateRemindersAsync(cancellationToken);
        if (result.Error is not null)
        {
            return Problem(result.Error);
        }

        return Ok(new SimulateReminderResponseModel
        {
            CheckedCount = result.CheckedCount,
            SentCount = result.SentCount,
            OverdueCount = result.CheckedCount,
            Message = result.Message
        });
    }

    private TResponse GetResponse<TDomain, TResponse>(IExecutionResult result)
        where TResponse : ResponseModel
    {
        var response = mappingRegistry.Map<TResponse>(result.GetResult<TDomain>());

        if (!result.IsSuccess)
            response.Notification = result.GetErrors();

        return response;
    }

    private TResponse GetResponse<TResponse>(IExecutionResult result)
        where TResponse : ResponseModel, new()
    {
        var response = new TResponse();

        if(!result.IsSuccess)
        {
            response.Notification = result.GetErrors();
        }

        return response;
    }
}
