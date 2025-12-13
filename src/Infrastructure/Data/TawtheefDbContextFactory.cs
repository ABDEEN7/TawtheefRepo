using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Serilog;
using Tawtheef.Infrastructure.Data;

public class TawtheefDbContextFactory : IDesignTimeDbContextFactory<TawtheefDbContext>
{
    public TawtheefDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TawtheefDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\MSSQLLocalDB;Database=TawtheefDB;Trusted_Connection=True");

        // Provide a dummy logger for design-time creation
        var logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        return new TawtheefDbContext(optionsBuilder.Options, logger);
    }
}
