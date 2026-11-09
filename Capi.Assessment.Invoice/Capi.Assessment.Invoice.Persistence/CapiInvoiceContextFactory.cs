using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Capi.Assessment.Invoice.Persistence
{
    public class CapiInvoiceContextFactory : IDesignTimeDbContextFactory<CapiInvoiceContext>
    {
        public CapiInvoiceContext CreateDbContext(string[] args)
        {
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{envName}.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<CapiInvoiceContext>();
            optionsBuilder.UseSqlServer(configuration["DataConnection:Database"]);

            return new CapiInvoiceContext(optionsBuilder.Options);
        }
    }
}
