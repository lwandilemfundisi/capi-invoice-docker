using Capi.Assessment.Invoice.Api.Mappers.Abstract;
using Capi.Assessment.Invoice.Api.Models.ResponseModels;
using XFrame.Common.Extensions;

namespace Capi.Assessment.Invoice.Api.Mappers
{
    public class MapToGetInvoiceResponseModel(IEnumerable<Domain.InvoiceDomainModel.Invoice> invoices) : AbstractMap<IReadOnlyCollection<InvoiceModel>>
    {
        public override IReadOnlyCollection<InvoiceModel> Map()
        {
            if (invoices.HasItems())
            { 
                return [.. invoices.Select(i => new InvoiceModel
                {
                    Id = i.Id.ToString(),
                    Client = i.Client,
                    Email = i.Email,
                    Amount = i.Amount,
                    Currency = i.Currency,
                    IssuedDate = i.IssuedDate,
                    DueDate = i.DueDate,
                    Status = i.Status,
                    Description = i.Description,
                    PaidAt = i.PaidAt
                })];
            }

            return [];
        }
    }
}