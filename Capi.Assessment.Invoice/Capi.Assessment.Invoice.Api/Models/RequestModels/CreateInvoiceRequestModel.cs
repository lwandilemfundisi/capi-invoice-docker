using System.ComponentModel.DataAnnotations;

namespace Capi.Assessment.Invoice.Api.Models.RequestModels
{
    public class CreateInvoiceRequestModel
    {
        public string Client { get; set; }

        public string Email { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public DateOnly IssuedDate { get; set; }
        public DateOnly DueDate { get; set; }

        public string Status { get; set; }

        public string Description { get; set; }

        public DateTimeOffset? PaidAt { get; set; }
    }
}
