using XFrame.Common.Extensions;
using XFrame.Notifications;
using XFrame.Specifications;

namespace Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Specifications
{
    public class CreateInvoiceSpecification : Specification<Invoice>
    {
        protected override Notification IsNotSatisfiedBecause(Invoice obj)
        {
            List<Message> errors = [];

            void AddError(string reason) =>
                errors.Add(new Message($"Could not create invoice: {reason}", SeverityType.Critical));

            if (obj.Client is null) AddError("Client is required");
            if (obj.Email.IsNullOrEmpty()) AddError("Email is required");
            if (obj.Currency.IsNullOrEmpty()) AddError("Currency is required");
            if (obj.Status.IsNullOrEmpty()) AddError("Status is required");
            if (obj.Description.IsNullOrEmpty()) AddError("Description is required");
            if (obj.Amount is < 0.01m or > 999999999.99m) AddError("Amount is invalid");
            if (obj.IssuedDate < DateOnly.FromDateTime(DateTime.Today)) AddError("Issued date is invalid");
            if (obj.DueDate < obj.IssuedDate) AddError("Due date is invalid");

            return errors.Count > 0
                ? Notification.Create(errors)
                : Notification.CreateEmpty();
        }
    }
}