using XFrame.Aggregates.Queries;
using XFrame.Persistence;
using XFrame.Persistence.EFCore.Extensions;
using XFrame.Persistence.EFCore.Queries.CriteriaQueries;
using XFrame.Persistence.EFCore.Queries.Filtering;

namespace Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Queries
{
    #region Query

    public class GetInvoicesQuery(IList<string> status, DateTime today) : EFCoreCriteriaDomainQuery<Invoice>, IQuery<IEnumerable<Invoice>>
    {
        #region Properties

        public IList<string> Status { get; } = status;

        public DateTime TodaysDate { get; } = today;

        #endregion

        #region Overrides

        protected override void OnBuildDomainCriteria(EFCoreDomainCriteria domainCriteria)
        {
            domainCriteria.BuildCriteria<Invoice>(criteria => criteria
            .When(Status.Any(), x => x.NotIn(i => i.Status, Status))
            .When(TodaysDate != default, x => x.LessThan(i => i.DueDate, TodaysDate)));
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
