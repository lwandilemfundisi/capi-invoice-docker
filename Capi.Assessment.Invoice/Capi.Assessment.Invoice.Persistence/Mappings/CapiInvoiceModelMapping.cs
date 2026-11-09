using Capi.Assessment.Invoice.Domain.InvoiceDomainModel;
using Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Entities;
using Capi.Assessment.Invoice.Persistence.ValueObjectConverters;
using Microsoft.EntityFrameworkCore;
using XFrame.Aggregates.Events;

namespace Capi.Assessment.Invoice.Persistence.Mappings
{
    public static class CapiInvoiceModelMapping
    {
        public static ModelBuilder CapiInvoiceModelMap(this ModelBuilder modelBuilder)
        {
            modelBuilder
            .Entity<Domain.InvoiceDomainModel.Invoice>()
            .Property(o => o.Id)
            .HasConversion(new IdentityValueConverter<InvoiceId>());

            modelBuilder
            .Entity<Reminder>()
            .Property(o => o.Id)
            .HasConversion(new IdentityValueConverter<ReminderId>());

            modelBuilder
            .Entity<Reminder>()
            .Property(o => o.InvoiceId)
            .HasConversion(new IdentityValueConverter<InvoiceId>());

            modelBuilder
                .Entity<Reminder>()
                .HasOne<Domain.InvoiceDomainModel.Invoice>()
                .WithMany(i => i.Reminders)
                .HasForeignKey(r => r.InvoiceId);

            return modelBuilder;
        }
    }
}
