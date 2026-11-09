using Capi.Assessment.Invoice.Api.Mappers.Abstract;
using Capi.Assessment.Invoice.Api.Models.ResponseModels;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel;
using XFrame.Common.Extensions;

namespace Capi.Assessment.Invoice.Api.Mappers
{
    public class MapToGetRemindersResponseModel(IEnumerable<Domain.InvoiceDomainModel.Entities.Reminder> reminders)
        : AbstractMap<IReadOnlyCollection<ReminderModel>>
    {
        public override IReadOnlyCollection<ReminderModel> Map()
        {
            if (reminders.HasItems())
            {
                return [.. reminders.Select(r => new ReminderModel
                {
                    Client = r.Client,
                    Message = r.Message,
                    SentAt = r.SentAt,
                    Type = r.Type,
                    Id = r.Id.ToString(),

                })];
            }

            return [];
        }
    }
}
