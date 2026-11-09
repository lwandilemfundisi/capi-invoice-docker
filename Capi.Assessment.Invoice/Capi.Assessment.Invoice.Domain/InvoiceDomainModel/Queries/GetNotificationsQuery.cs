using Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Entities;
using XFrame.Aggregates.Queries;
using XFrame.Persistence;
using XFrame.Persistence.EFCore.Queries.CriteriaQueries;

namespace Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Queries
{
    #region Query

    public class GetNotificationsQuery : EFCoreCriteriaDomainQuery<Reminder>, IQuery<IEnumerable<Reminder>>
    {
    }

    #endregion

    #region Query Handler

    public class GetNotificationsQueryHandler(IPersistenceFactory persistenceFactory)
        : EFCoreCriteriaDomainQueryHandler<Reminder>(persistenceFactory), IQueryHandler<GetNotificationsQuery, IEnumerable<Reminder>>
    {
        public Task<IEnumerable<Reminder>> ExecuteQueryAsync(GetNotificationsQuery query, CancellationToken cancellationToken)
        {
            return FindAll(query);
        }
    }

    #endregion
}
