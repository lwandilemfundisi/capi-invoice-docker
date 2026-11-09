using Microsoft.VisualBasic;
using XFrame.Aggregates.Queries;
using XFrame.Common.Extensions;
using XFrame.Persistence;
using XFrame.Persistence.EFCore.Queries.CriteriaQueries;
using XFrame.Persistence.EFCore.Queries.Filtering;
using XFrame.Persistence.Extensions;
using XFrame.Persistence.Queries.Filterings;

namespace Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Queries
{
    #region Query

    public class GetInvoicesQuery(IList<string> status, DateOnly today) : EFCoreCriteriaDomainQuery<Invoice>, IQuery<IEnumerable<Invoice>>
    {
        #region Properties

        public IList<string> Status { get; } = status;

        public DateOnly TodaysDate { get; } = today;

        #endregion

        #region Overrides

        protected override void OnBuildDomainCriteria(EFCoreDomainCriteria domainCriteria)
        {
            if(Status.HasItems() && TodaysDate != default)
            {
                domainCriteria.SafeAnd(new AndFilter(
                    new EqualityFilter("Status", Status, FilterType.NotIn),
                    new EqualityFilter("DueDate", TodaysDate, FilterType.LessThan)));
            }
        }

        #endregion

    }

    #endregion

    #region Query Handler

    public class GetInvoicesQueryHandler(IPersistenceFactory persistenceFactory) 
        : EFCoreCriteriaDomainQueryHandler<Invoice>(persistenceFactory), IQueryHandler<GetInvoicesQuery, IEnumerable<Invoice>>
    {

        #region IQuery Handler Members

        public Task<IEnumerable<Invoice>> ExecuteQueryAsync(
            GetInvoicesQuery query, 
            CancellationToken cancellationToken)
        {
            return FindAll(query);
        }

        #endregion
    }

    #endregion
}
