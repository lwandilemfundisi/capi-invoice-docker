using Capi.Assessment.Invoice.Persistence.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Capi.Assessment.Invoice.Persistence
{
    public class CapiInvoiceContext(DbContextOptions<CapiInvoiceContext> options) : DbContext(options)
    {
        #region Override Methods

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.CapiInvoiceModelMap();
        }

        #endregion
    }
}
