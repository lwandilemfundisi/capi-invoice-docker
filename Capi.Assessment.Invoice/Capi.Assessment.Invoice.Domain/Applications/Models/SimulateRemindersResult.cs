using System;
using System.Collections.Generic;
using System.Text;

namespace Capi.Assessment.Invoice.Domain.Applications.Models
{
    public sealed record SimulateRemindersResult(string Error, int CheckedCount, int SentCount, string Message);
}
