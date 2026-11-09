using Newtonsoft.Json;
using XFrame.Ids;
using XFrame.ValueObjects.SingleValueObjects;

namespace Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Entities
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class ReminderId : Identity<ReminderId>
    {
        #region Constructors

        public ReminderId(string id)
            : base(id)
        {

        }

        #endregion
    }
}
