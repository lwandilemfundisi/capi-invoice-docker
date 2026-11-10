using Capi.Assessment.Invoice.Domain.InvoiceDomainModel;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Entities;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Queries;
using XFrame.Aggregates.Queries;
using Xunit;

namespace Capi.Assessment.Invoice.Api.Tests;

public sealed class QueryTests
{
    [Fact]
    public void GetInvoicesQuery_preserves_statuses_and_date_filter()
    {
        IList<string> statuses = ["sent", "overdue"];
        var today = new DateTime(2026, 11, 10);

        var query = new GetInvoicesQuery(statuses, today);

        Assert.Same(statuses, query.Status);
        Assert.Equal(today, query.TodaysDate);
        Assert.IsAssignableFrom<IQuery<IEnumerable<Domain.InvoiceDomainModel.Invoice>>>(query);
    }

    [Fact]
    public void GetInvoicesQuery_accepts_empty_statuses_and_default_date()
    {
        IList<string> statuses = [];

        var query = new GetInvoicesQuery(statuses, default);

        Assert.Empty(query.Status);
        Assert.Equal(default, query.TodaysDate);
    }

    [Fact]
    public void GetNotificationsQuery_is_a_query_for_reminders()
    {
        var query = new GetNotificationsQuery();

        Assert.IsAssignableFrom<IQuery<IEnumerable<Reminder>>>(query);
    }
}
