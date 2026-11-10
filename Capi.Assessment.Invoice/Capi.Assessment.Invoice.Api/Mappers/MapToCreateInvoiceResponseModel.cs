using Capi.Assessment.Invoice.Api.Mappers.Abstract;
using Capi.Assessment.Invoice.Api.Models.ResponseModels;

namespace Capi.Assessment.Invoice.Api.Mappers
{
    public class MapToCreateInvoiceResponseModel(Domain.InvoiceDomainModel.Invoice invoice) : AbstractMap<CreateInvoiceModel>
    {
        public override CreateInvoiceModel Map()
        {
            if (invoice is not null)
            {
                return new CreateInvoiceModel
                {
                    Id = invoice.Id.ToString(),
                    Client = invoice.Client,
                    Email = invoice.Email,
                    Amount = invoice.Amount,
                    Currency = invoice.Currency,
                    IssuedDate = invoice.IssuedDate,
                    DueDate = invoice.DueDate,
                    Status = invoice.Status,
                    Description = invoice.Description,
                    PaidAt = invoice.PaidAt
                };
            }

            return new CreateInvoiceModel();
        }
    }
}