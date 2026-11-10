using Capi.Assessment.Invoice.Domain;
using Capi.Assessment.Invoice.Domain.Extensions;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Commands;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Rules.Contexts;
using Microsoft.Extensions.DependencyInjection;
using XFrame.Notifications.Extensions;
using XFrame.Rules;
using Xunit;

namespace Capi.Assessment.Invoice.Api.Tests;

public sealed class CreateInvoiceCommandRulesTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IValidator _validator;

    public CreateInvoiceCommandRulesTests()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IValidator, Validator>();
        services.ConfigureCapiAssessmentInvoiceDomain();

        _serviceProvider = services.BuildServiceProvider();
        _validator = _serviceProvider.GetRequiredService<IValidator>();
    }

    [Theory]
    [InlineData(null, "client@example.com", "ZAR", "An invoice")]
    [InlineData("", "client@example.com", "ZAR", "An invoice")]
    [InlineData("Client", null, "ZAR", "An invoice")]
    [InlineData("Client", "client@example.com", null, "An invoice")]
    [InlineData("Client", "client@example.com", "ZAR", null)]
    public async Task Required_fields_must_have_values(
        string client,
        string email,
        string currency,
        string description)
    {
        var command = CreateCommand(
            client: client,
            email: email,
            currency: currency,
            description: description);

        var result = await Validate(command);

        Assert.True(result.HasErrors());
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("client@")]
    public async Task Email_must_be_valid(string email)
    {
        var result = await Validate(CreateCommand(email: email));

        Assert.True(result.HasErrors());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1000000.01)]
    public async Task Amount_must_be_between_point_zero_one_and_one_million(decimal amount)
    {
        var result = await Validate(CreateCommand(amount: amount));

        Assert.True(result.HasErrors());
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(1000000)]
    public async Task Amount_range_includes_its_boundaries(decimal amount)
    {
        var result = await Validate(CreateCommand(amount: amount));

        Assert.False(result.HasErrors());
    }

    [Fact]
    public async Task Issue_date_cannot_be_before_today()
    {
        var yesterday = DateTime.Today.AddDays(-1);
        var result = await Validate(CreateCommand(issuedDate: yesterday, dueDate: yesterday));

        Assert.True(result.HasErrors());
    }

    [Fact]
    public async Task Due_date_cannot_be_before_today()
    {
        var yesterday = DateTime.Today.AddDays(-1);
        var result = await Validate(CreateCommand(issuedDate: DateTime.Today, dueDate: yesterday));

        Assert.True(result.HasErrors());
    }

    [Fact]
    public async Task Due_date_cannot_be_before_issue_date()
    {
        var today = DateTime.Today;
        var result = await Validate(CreateCommand(
            issuedDate: today.AddDays(2),
            dueDate: today.AddDays(1)));

        Assert.True(result.HasErrors());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(201)]
    public async Task Description_must_be_between_two_and_two_hundred_characters(int length)
    {
        var result = await Validate(CreateCommand(description: new string('x', length)));

        Assert.True(result.HasErrors());
    }

    [Theory]
    [InlineData(2)]
    [InlineData(200)]
    public async Task Description_range_includes_its_boundaries(int length)
    {
        var result = await Validate(CreateCommand(description: new string('x', length)));

        Assert.False(result.HasErrors());
    }

    [Fact]
    public async Task Valid_invoice_command_passes_all_rules()
    {
        var result = await Validate(CreateCommand());

        Assert.False(result.HasErrors());
    }

    public void Dispose() => _serviceProvider.Dispose();

    private Task<XFrame.Notifications.Notification> Validate(CreateInvoiceCommand command) =>
        _validator.ValidateAsync<CreateInvoiceCommand, InvoiceValidationContext>(command);

    private static CreateInvoiceCommand CreateCommand(
        string client = "Client",
        string email = "client@example.com",
        decimal amount = 250m,
        string currency = "ZAR",
        DateTime? issuedDate = null,
        DateTime? dueDate = null,
        string status = "sent",
        string description = "An invoice")
    {
        var today = DateTime.Today;

        return new CreateInvoiceCommand(
            InvoiceId.New,
            client,
            email,
            amount,
            currency,
            issuedDate ?? today,
            dueDate ?? today,
            status,
            description);
    }
}
