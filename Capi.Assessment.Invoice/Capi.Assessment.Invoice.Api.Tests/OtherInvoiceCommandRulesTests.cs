using Capi.Assessment.Invoice.Domain;
using Capi.Assessment.Invoice.Domain.Extensions;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Commands;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Rules.Contexts;
using Microsoft.Extensions.DependencyInjection;
using XFrame.Notifications;
using XFrame.Notifications.Extensions;
using XFrame.Rules;
using Xunit;

namespace Capi.Assessment.Invoice.Api.Tests;

public sealed class OtherInvoiceCommandRulesTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IValidator _validator;

    public OtherInvoiceCommandRulesTests()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IValidator, Validator>();
        services.ConfigureCapiAssessmentInvoiceDomain();

        _serviceProvider = services.BuildServiceProvider();
        _validator = _serviceProvider.GetRequiredService<IValidator>();
    }

    [Fact]
    public async Task Mark_paid_command_passes_its_required_rules()
    {
        var command = new UpdateInvoiceStatusPaidCommand(InvoiceId.New);

        var result = await Validate(command);

        Assert.Equal("paid", command.Status);
        Assert.False(result.HasErrors());
    }

    [Fact]
    public async Task Mark_overdue_command_passes_its_required_rules()
    {
        var command = new UpdateInvoiceStatusOverdueCommand(InvoiceId.New);

        var result = await Validate(command);

        Assert.Equal("overdue", command.Status);
        Assert.False(result.HasErrors());
    }

    [Theory]
    [InlineData(null, "Reminder message", "reminder")]
    [InlineData("", "Reminder message", "reminder")]
    [InlineData("Client", null, "reminder")]
    [InlineData("Client", "", "reminder")]
    [InlineData("Client", "Reminder message", null)]
    [InlineData("Client", "Reminder message", "")]
    public async Task Reminder_required_fields_must_have_values(string client, string message, string type)
    {
        var command = CreateReminder(client, message, type);

        var result = await Validate(command);

        Assert.True(result.HasErrors());
    }

    [Fact]
    public async Task Reminder_command_passes_when_required_fields_are_present()
    {
        var result = await Validate(CreateReminder("Client", "Reminder message", "reminder"));

        Assert.False(result.HasErrors());
    }

    public void Dispose() => _serviceProvider.Dispose();

    private Task<Notification> Validate<TCommand>(TCommand command) =>
        _validator.ValidateAsync<TCommand, InvoiceValidationContext>(command);

    private static CreateInvoiceReminderCommand CreateReminder(string client, string message, string type) =>
        new(InvoiceId.New, client, message, DateTimeOffset.Now, type);
}
