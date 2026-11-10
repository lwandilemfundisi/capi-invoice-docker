using Capi.Assessment.Invoice.Api.Mappers.Abstract;
using Capi.Assessment.Invoice.Api.Models.ResponseModels.Abstract;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel;

namespace Capi.Assessment.Invoice.Api.Models.ResponseModels
{
    public class GetInvoicesResponseModel: ResponseModel<IReadOnlyCollection<InvoiceModel>>
    {
    }

    [MapsFrom(typeof(Domain.InvoiceDomainModel.Invoice))]
    public class InvoiceModel
    {
        public string Id { get; set; }
        public string Client { get; set; }
        public string Email { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public DateTimeOffset? PaidAt { get; set; }
    }
}
