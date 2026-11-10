using Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Commands;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Rules.Contexts;
using XFrame.Common.Extensions;
using XFrame.Notifications;
using XFrame.Rules.Validations;
using XFrame.Rules.Validations.Attributes;
using XFrame.Rules.Validations.Common;

namespace Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Rules
{
    [ValidationContext(typeof(InvoiceValidationContext))]
    [Validation(typeof(CreateInvoiceCommand))]
    [ValidationProperty(nameof(CreateInvoiceCommand.Client))]
    [ValidationProperty(nameof(CreateInvoiceCommand.Email))]
    [ValidationProperty(nameof(CreateInvoiceCommand.Amount))]
    [ValidationProperty(nameof(CreateInvoiceCommand.Currency))]
    [ValidationProperty(nameof(CreateInvoiceCommand.IssuedDate))]
    [ValidationProperty(nameof(CreateInvoiceCommand.DueDate))]
    [ValidationProperty(nameof(CreateInvoiceCommand.Status))]
    [ValidationProperty(nameof(CreateInvoiceCommand.Description))]
    public class CreateInvoiceCommandRequiredRules : RequiredValidation<CreateInvoiceCommand>
    {
    }

    [ValidationContext(typeof(InvoiceValidationContext))]
    [Validation(typeof(CreateInvoiceCommand))]
    [ValidationProperty(nameof(CreateInvoiceCommand.Email))]
    public class CreateInvoiceCommandEmailRules : EmailPropertyValidation<CreateInvoiceCommand>
    {
        protected override bool OnMustValidate()
        {
            return PropertyHasValue();
        }
    }

    [ValidationContext(typeof(InvoiceValidationContext))]
    [Validation(typeof(CreateInvoiceCommand))]
    [ValidationProperty(nameof(CreateInvoiceCommand.Amount))]
    public class CreateInvoiceCommandAmountRules : DecimalRangeValidation<CreateInvoiceCommand>
    {
        protected override bool OnMustValidate()
        {
            return PropertyHasValue();
        }

        protected override decimal? OnGetMinimum()
        {
            return 0.01m;
        }

        protected override decimal? OnGetMaximum()
        {
            return 1000000m;
        }
    }

    [ValidationContext(typeof(InvoiceValidationContext))]
    [Validation(typeof(CreateInvoiceCommand))]
    [ValidationProperty(nameof(CreateInvoiceCommand.IssuedDate))]
    public class CreateInvoiceCommandIssueDateRules : DateRangeValidation<CreateInvoiceCommand>
    {
        protected override bool OnMustValidate()
        {
            return PropertyHasValue();
        }

        protected override DateTime OnGetMinimum()
        {
            return DateTime.Today;
        }

        protected override Task<Notification> OnValidate(CancellationToken cancellationToken)
        {
            Notification notification = Notification.CreateEmpty();
            DateTime? dateTime = PropertyValue as DateTime?;
            if (dateTime.IsNotNull())
            {
                DateTime value = OnGetMinimum();
                if (dateTime > OnGetMaximum() || dateTime < value)
                {
                    notification.AddMessage(OnCreateMessage());
                }
            }

            return Task.FromResult(notification);
        }
    }


    [ValidationContext(typeof(InvoiceValidationContext))]
    [Validation(typeof(CreateInvoiceCommand))]
    [ValidationProperty(nameof(CreateInvoiceCommand.DueDate))]
    public class CreateInvoiceCommandDueDateRules : DateRangeValidation<CreateInvoiceCommand>
    {
        protected override bool OnMustValidate()
        {
            return PropertyHasValue();
        }

        protected override DateTime OnGetMinimum()
        {
            return DateTime.Today;
        }
    }

    [ValidationContext(typeof(InvoiceValidationContext))]
    [Validation(typeof(CreateInvoiceCommand))]
    [ValidationProperty(nameof(CreateInvoiceCommand.DueDate))]
    public class CreateInvoiceCommandDueDateCannotBeGreaterThanIssuedDateRules : Validation<CreateInvoiceCommand>
    {
        protected override bool OnMustValidate()
        {
            return PropertyHasValue();
        }

        protected override Task<Notification> OnValidate(CancellationToken cancellationToken)
        {
            var notification = Notification.CreateEmpty();

            if ((DateTime)PropertyValue < Instance.IssuedDate)
            {
                notification.AddError(new Message("Due date cannot be less than issued date."));
            }

            return Task.FromResult(notification);
        }
    }

    [ValidationContext(typeof(InvoiceValidationContext))]
    [Validation(typeof(CreateInvoiceCommand))]
    [ValidationProperty(nameof(CreateInvoiceCommand.Description))]
    public class CreateInvoiceCommandDescriptionRules : StringRangeValidation<CreateInvoiceCommand>
    {
        protected override bool OnMustValidate()
        {
            return PropertyHasValue();
        }

        protected override int OnGetMaximum()
        {
            return 200;
        }

        protected override int OnGetMinimum()
        {
            return 2;
        }
    }
}