using Capi.Assessment.Invoice.Domain;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Commands;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Entities;
using Microsoft.Extensions.DependencyInjection;
using XFrame.Rules;
using Xunit;

namespace Capi.Assessment.Invoice.Api.Tests;

public sealed class CommandHandlerTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IValidator _validator;

    public CommandHandlerTests()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IValidator, Validator>();
        services.ConfigureCapiAssessmentInvoiceDomain();

        _serviceProvider = services.BuildServiceProvider();
        _validator = _serviceProvider.GetRequiredService<IValidator>();
    }

    [Fact]
    public async Task CreateInvoiceCommandHandler_creates_invoice_when_command_is_valid()
    {
        var aggregate = new Domain.InvoiceDomainModel.Invoice(InvoiceId.New);
        var command = CreateInvoiceCommand();
        var handler = new CreateInvoiceCommandHandler(_validator);

        var result = await handler.ExecuteAsync(aggregate, command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(command.Client, aggregate.Client);
        Assert.Equal(command.Email, aggregate.Email);
        Assert.Equal(command.Amount, aggregate.Amount);
        Assert.Equal(command.Currency, aggregate.Currency);
        Assert.Equal(command.IssuedDate, aggregate.IssuedDate);
        Assert.Equal(command.DueDate, aggregate.DueDate);
        Assert.Equal(command.Status, aggregate.Status);
        Assert.Equal(command.Description, aggregate.Description);
    }

    [Fact]
    public async Task CreateInvoiceCommandHandler_returns_failure_when_command_is_invalid()
    {
        var aggregate = new Domain.InvoiceDomainModel.Invoice(InvoiceId.New);
        var command = CreateInvoiceCommand(amount: 0);
        var handler = new CreateInvoiceCommandHandler(_validator);

        var result = await handler.ExecuteAsync(aggregate, command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Null(aggregate.Client);
    }

    [Fact]
    public async Task UpdateInvoiceStatusPaidCommandHandler_marks_invoice_paid()
    {
        var aggregate = CreateExistingInvoice();
        var command = new UpdateInvoiceStatusPaidCommand(aggregate.Id);
        var handler = new UpdateInvoiceStatusPaidCommandHandler();

        var result = await handler.ExecuteAsync(aggregate, command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("paid", aggregate.Status);
        Assert.Equal(command.PaidAt, aggregate.PaidAt);
    }

    [Fact]
    public async Task UpdateInvoiceStatusOverdueCommandHandler_marks_invoice_overdue()
    {
        var aggregate = CreateExistingInvoice();
        var command = new UpdateInvoiceStatusOverdueCommand(aggregate.Id);
        var handler = new UpdateInvoiceStatusOverdueCommandHandler();

        var result = await handler.ExecuteAsync(aggregate, command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("overdue", aggregate.Status);
    }

    [Fact]
    public async Task CreateInvoiceReminderCommandHandler_adds_reminder_to_invoice()
    {
        var aggregate = CreateExistingInvoice();
        var sentAt = DateTimeOffset.UtcNow;
        var command = new CreateInvoiceReminderCommand(
            aggregate.Id,
            "Client",
            "Please pay this invoice.",
            sentAt,
            "reminder");
        var handler = new CreateInvoiceReminderCommandHandler();

        var result = await handler.ExecuteAsync(aggregate, command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var reminder = Assert.Single(aggregate.Reminders);
        Assert.Equal("Client", reminder.Client);
        Assert.Equal("Please pay this invoice.", reminder.Message);
        Assert.Equal(sentAt, reminder.SentAt);
        Assert.Equal("reminder", reminder.Type);
    }

    public void Dispose() => _serviceProvider.Dispose();

    private static CreateInvoiceCommand CreateInvoiceCommand(decimal amount = 250m)
    {
        var today = DateTime.Today;
        return new CreateInvoiceCommand(
            InvoiceId.New,
            "Client",
            "client@example.com",
            amount,
            "ZAR",
            today,
            today,
            "sent",
            "An invoice");
    }

    private static Domain.InvoiceDomainModel.Invoice CreateExistingInvoice() => new(InvoiceId.New)
    {
        Client = "Client",
        Email = "client@example.com",
        Amount = 250m,
        Currency = "ZAR",
        IssuedDate = DateTime.Today,
        DueDate = DateTime.Today,
        Status = "sent",
        Description = "An invoice",
        Reminders = new List<Reminder>(),
        Version = 1
    };
}
