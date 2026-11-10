using Capi.Assessment.Invoice.Api.Controllers;
using Capi.Assessment.Invoice.Api.Mappers.Abstract;
using Capi.Assessment.Invoice.Api.Models.RequestModels;
using Capi.Assessment.Invoice.Api.Models.ResponseModels;
using Capi.Assessment.Invoice.Api.Models.ResponseModels.Abstract;
using Capi.Assessment.Invoice.Domain.Applications;
using Capi.Assessment.Invoice.Domain.Applications.Models;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel;
using Microsoft.AspNetCore.Mvc;
using XFrame.Aggregates.ExecutionResults;
using Xunit;

namespace Capi.Assessment.Invoice.Api.Tests;

public sealed class ControllerTests
{
    private readonly FakeInvoiceApplication _application = new();
    private readonly InvoiceController _controller;

    public ControllerTests()
    {
        var mappingRegistry = new MappingRegistry(
            typeof(ResponseModel<>),
            typeof(CreateInvoiceResponseModel).Assembly);
        _controller = new InvoiceController(_application, mappingRegistry);
    }

    [Fact]
    public async Task CreateInvoice_passes_request_fields_to_application_and_maps_result()
    {
        var issuedDate = new DateTime(2026, 11, 10);
        var dueDate = issuedDate.AddDays(30);
        _application.CreateResult = ExecutionResult.Success(CreateInvoiceAggregate());

        var response = await _controller.CreateInvoice(new CreateInvoiceRequestModel
        {
            Client = "Acme",
            Email = "billing@acme.example",
            Amount = 123.45m,
            Currency = "ZAR",
            IssuedDate = issuedDate,
            DueDate = dueDate,
            Status = "draft",
            Description = "Consulting"
        }, CancellationToken.None);

        Assert.Equal("Acme", _application.CreateClient);
        Assert.Equal("billing@acme.example", _application.CreateEmail);
        Assert.Equal(123.45m, _application.CreateAmount);
        Assert.Equal("ZAR", _application.CreateCurrency);
        Assert.Equal(issuedDate, _application.CreateIssuedDate);
        Assert.Equal(dueDate, _application.CreateDueDate);
        Assert.Equal("Consulting", _application.CreateDescription);
        Assert.Equal("Client", response.Result.Client);
        Assert.Equal("client@example.com", response.Result.Email);
    }

    [Fact]
    public async Task ListInvoices_passes_filters_and_maps_application_results()
    {
        var today = new DateTime(2026, 11, 10);
        _application.Invoices = null;

        var actionResult = await _controller.ListInvoices(
            new GetInvoicesRequestModel { Status = "overdue", TodaysDate = today },
            CancellationToken.None);

        var response = Assert.IsType<OkObjectResult>(actionResult.Result);
        Assert.Equal("overdue", _application.InvoiceStatus);
        Assert.Equal(today, _application.InvoicesToday);
        Assert.Null(response.Value);
    }

    [Fact]
    public async Task UpdateInvoiceStatus_passes_route_id_to_application()
    {
        _application.UpdateResult = ExecutionResult.Success();

        await _controller.UpdateInvoiceStatus("invoice-123", CancellationToken.None);

        Assert.Equal("invoice-123", _application.UpdatedInvoiceId);
    }

    [Fact]
    public async Task GetNotifications_maps_application_reminders()
    {
        _application.Reminders = null;

        var actionResult = await _controller.GetNotifications(CancellationToken.None);

        var response = Assert.IsType<OkObjectResult>(actionResult.Result);
        Assert.Null(response.Value);
        Assert.True(_application.GetRemindersCalled);
    }

    [Fact]
    public async Task SimulateReminder_returns_counts_and_message()
    {
        _application.SimulationResult = new SimulateRemindersResult(null, 5, 2, "Complete");

        var actionResult = await _controller.SimulateReminder(CancellationToken.None);

        var response = Assert.IsType<OkObjectResult>(actionResult.Result);
        var model = Assert.IsType<SimulateReminderResponseModel>(response.Value);
        Assert.Equal(5, model.CheckedCount);
        Assert.Equal(2, model.SentCount);
        Assert.Equal(5, model.OverdueCount);
        Assert.Equal("Complete", model.Message);
    }

    [Fact]
    public async Task SimulateReminder_returns_problem_when_application_reports_error()
    {
        _application.SimulationResult = new SimulateRemindersResult("Failed", 0, 0, null);

        var actionResult = await _controller.SimulateReminder(CancellationToken.None);

        var problem = Assert.IsType<ObjectResult>(actionResult.Result);
        Assert.Equal(500, problem.StatusCode);
        Assert.Equal("Failed", Assert.IsType<ProblemDetails>(problem.Value).Detail);
    }

    private static Domain.InvoiceDomainModel.Invoice CreateInvoiceAggregate()
    {
        var invoice = new Domain.InvoiceDomainModel.Invoice(InvoiceId.New)
        {
            Client = "Client",
            Email = "client@example.com",
            Amount = 50,
            Currency = "ZAR",
            IssuedDate = new DateTime(2026, 11, 10),
            DueDate = new DateTime(2026, 12, 10),
            Status = "sent",
            Description = "An invoice"
        };

        return invoice;
    }

    private sealed class FakeInvoiceApplication : ICapiInvoiceApplication
    {
        public IExecutionResult CreateResult { get; set; }
        public IExecutionResult UpdateResult { get; set; }
        public IEnumerable<Domain.InvoiceDomainModel.Invoice> Invoices { get; set; } = [];
        public IEnumerable<Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Entities.Reminder> Reminders { get; set; } = [];
        public SimulateRemindersResult SimulationResult { get; set; } = new(null, 0, 0, null);

        public string CreateClient { get; private set; }
        public string CreateEmail { get; private set; }
        public decimal CreateAmount { get; private set; }
        public string CreateCurrency { get; private set; }
        public DateTime CreateIssuedDate { get; private set; }
        public DateTime CreateDueDate { get; private set; }
        public string CreateDescription { get; private set; }
        public string InvoiceStatus { get; private set; }
        public DateTime InvoicesToday { get; private set; }
        public string UpdatedInvoiceId { get; private set; }
        public bool GetRemindersCalled { get; private set; }

        public Task<IExecutionResult> CreateInvoiceAsync(
            string client, string email, decimal amount, string currency,
            DateTime issuedDate, DateTime dueDate, string description,
            CancellationToken cancellationToken)
        {
            CreateClient = client;
            CreateEmail = email;
            CreateAmount = amount;
            CreateCurrency = currency;
            CreateIssuedDate = issuedDate;
            CreateDueDate = dueDate;
            CreateDescription = description;
            return Task.FromResult(CreateResult);
        }

        public Task<IEnumerable<Domain.InvoiceDomainModel.Invoice>> GetInvoicesAsync(string status, DateTime today, CancellationToken cancellationToken)
        {
            InvoiceStatus = status;
            InvoicesToday = today;
            return Task.FromResult(Invoices);
        }

        public Task<IExecutionResult> MarkInvoicePaidAsync(string invoiceId, CancellationToken cancellationToken)
        {
            UpdatedInvoiceId = invoiceId;
            return Task.FromResult(UpdateResult);
        }

        public Task<IEnumerable<Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Entities.Reminder>> GetRemindersAsync(CancellationToken cancellationToken)
        {
            GetRemindersCalled = true;
            return Task.FromResult(Reminders);
        }

        public Task<SimulateRemindersResult> SimulateRemindersAsync(CancellationToken cancellationToken) =>
            Task.FromResult(SimulationResult);
    }
}
