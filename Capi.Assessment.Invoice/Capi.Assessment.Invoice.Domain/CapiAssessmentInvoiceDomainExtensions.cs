using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using XFrame.Aggregates.Commands.Extensions;
using XFrame.Aggregates.EventMetadata.Extentions;
using XFrame.Aggregates.Events.Extensions;
using XFrame.Aggregates.Queries.Extensions;
using XFrame.DomainContainers;

namespace Capi.Assessment.Invoice.Domain
{
    public static class CapiAssessmentInvoiceDomainExtensions
    {
        public static Assembly Assembly { get; } = typeof(CapiAssessmentInvoiceDomainExtensions).Assembly;

        public static IDomainContainer ConfigureCapiAssessmentInvoiceDomain(
            this IServiceCollection services)
        {
            return DomainContainer.New(services)
                .AddEvents(Assembly, null)
                .AddCommands(Assembly, null)
                .AddCommandHandlers(Assembly, null)
                .AddQueries(Assembly, null)
                .AddQueryHandlers(Assembly, null)
                .AddMetadataProviders(Assembly, null);
        }
    }
}
