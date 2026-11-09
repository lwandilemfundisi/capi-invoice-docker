namespace Capi.Assessment.Invoice.Api.Models.RequestModels
{
    public class GetInvoicesRequestModel
    {
        #region Properties

        public string Status { get; set; }

        public DateOnly TodaysDate { get; set; }

        #endregion 
    }
}
