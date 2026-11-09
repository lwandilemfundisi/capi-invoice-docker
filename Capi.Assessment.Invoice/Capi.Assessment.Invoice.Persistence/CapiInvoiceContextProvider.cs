using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using XFrame.Persistence.EFCore;

namespace Capi.Assessment.Invoice.Persistence
{
    public class CapiInvoiceContextProvider(IConfiguration configuration) : IDbContextProvider<CapiInvoiceContext>, IDisposable
    {
        #region Constructors

        private readonly DbContextOptions<CapiInvoiceContext> _options = new DbContextOptionsBuilder<CapiInvoiceContext>()
                .UseSqlServer(configuration["DataConnection:Database"])
                .Options;

        #endregion

        #region IDbContextProvider Member

        public CapiInvoiceContext CreateContext()
        {
            return new CapiInvoiceContext(_options);
        }

        #endregion

        #region IDisposable Member

        public void Dispose()
        {
        }

        #endregion
    }
}
