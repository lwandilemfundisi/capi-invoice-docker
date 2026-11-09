using System.ComponentModel.DataAnnotations;
using Capi.Assessment.Invoice.Api.Models.RequestModels;
using Xunit;

namespace Capi.Assessment.Invoice.Api.Tests;

public class CreateInvoiceRequestModelTests
{
    [Fact]
    public void Validate_ValidInvoice_ReturnsNoErrors()
    {
        var errors = Validate(ValidRequest());

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_InvalidFields_ReturnsFieldErrors()
    {
        var request = ValidRequest();
        request.Email = "not-an-email";
        request.Amount = 0;
        request.Currency = "US";
        request.DueDate = request.IssuedDate.AddDays(-1);

        var errors = Validate(request);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(request.Email)));
        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(request.Amount)));
        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(request.Currency)));
        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(request.DueDate)));
    }

    [Fact]
    public void Validate_MissingRequiredValues_ReturnsRequiredErrors()
    {
        var errors = Validate(new CreateInvoiceRequestModel());

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(CreateInvoiceRequestModel.Client)));
        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(CreateInvoiceRequestModel.Email)));
        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(CreateInvoiceRequestModel.IssuedDate)));
        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(CreateInvoiceRequestModel.DueDate)));
    }

    private static List<ValidationResult> Validate(CreateInvoiceRequestModel request)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(request, new ValidationContext(request), results, validateAllProperties: true);
        return results;
    }

    private static CreateInvoiceRequestModel ValidRequest() => new()
    {
        Client = "Example Client",
        Email = "billing@example.com",
        Amount = 100m,
        Currency = "ZAR",
        IssuedDate = new DateOnly(2026, 11, 6),
        DueDate = new DateOnly(2026, 11, 20),
        Status = "Pending",
        Description = "Assessment invoice"
    };
}
