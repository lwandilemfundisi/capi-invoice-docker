namespace Capi.Assessment.Invoice.Api.Models.ResponseModels
{
    public class SimulateReminderResponseModel
    {
        public int CheckedCount { get; set; }
        public int SentCount { get; set; }
        public int OverdueCount { get; set; }
        public string Message { get; set; }
    }
}
