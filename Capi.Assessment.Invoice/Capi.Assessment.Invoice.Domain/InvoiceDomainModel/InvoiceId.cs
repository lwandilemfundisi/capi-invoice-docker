using Newtonsoft.Json;
using XFrame.Ids;
using XFrame.ValueObjects.SingleValueObjects;

namespace Capi.Assessment.Invoice.Domain.InvoiceDomainModel
{

    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class InvoiceId : Identity<InvoiceId>
    {
        #region Constructors

        public InvoiceId(string value) 
            : base(value)
        {
        }

        #endregion
    }
}
