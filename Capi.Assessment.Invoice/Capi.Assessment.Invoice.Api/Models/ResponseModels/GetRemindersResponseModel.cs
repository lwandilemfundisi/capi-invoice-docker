using Capi.Assessment.Invoice.Api.Mappers.Abstract;
using Capi.Assessment.Invoice.Api.Models.ResponseModels.Abstract;

namespace Capi.Assessment.Invoice.Api.Models.ResponseModels
{
    public class GetRemindersResponseModel : ResponseModel<IReadOnlyCollection<ReminderModel>>
    {
    }

    [MapsFrom(typeof(Domain.InvoiceDomainModel.Entities.Reminder))]
    public class ReminderModel
    {
        public string Id { get; set; }
        public string Client { get; set; }
        public string Message { get; set; }
        public DateTimeOffset SentAt { get; set; }
        public string Type { get; set; }
        public string InvoiceId { get; set; }
    }
}
